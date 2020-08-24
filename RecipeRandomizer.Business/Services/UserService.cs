using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Business.Models.Nomenclature;
using RecipeRandomizer.Business.Utils.Exceptions;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities.Identity;
using BC = BCrypt.Net.BCrypt;

namespace RecipeRandomizer.Business.Services
{
    public class UserService : IUserService
    {

        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly string _jwtSecret;
        private readonly IEmailService _emailService;

        public UserService(RRContext context, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
            _jwtSecret = configuration.GetValue<string>("JWTSecret");
            _emailService = emailService;
        }

        public User Authenticate(AuthRequest model, string ipAddress)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(x => x.Email == model.Email);

            // return null if user not found
            if (user == null || !user.IsVerified || !BC.Verify(model.Password, user.PasswordHash))
                throw new BadRequestException("Email or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // save refresh token
            user.RefreshTokens.Add(refreshToken);
            _userRepository.Update(user);
            _userRepository.SaveChanges();

            var authenticatedUser = _mapper.Map<User>(user);
            authenticatedUser.JwtToken = jwtToken;
            authenticatedUser.RefreshToken = refreshToken.Token;

            return authenticatedUser;
        }

        public User RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, user) = GetRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.RevokedOn = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _userRepository.Update(user);
            _userRepository.SaveChanges();

            // generate new jwt
            var jwtToken = GenerateJwtToken(user);

            var authenticatedUser = _mapper.Map<User>(user);
            authenticatedUser.JwtToken = jwtToken;
            authenticatedUser.RefreshToken = refreshToken.Token;

            return authenticatedUser;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, user) = GetRefreshToken(token);

            // revoke token and save
            refreshToken.RevokedOn = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _userRepository.Update(user);
            _userRepository.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            if(!model.HasAcceptedTerms)
                throw new BadRequestException("Terms and services have not been accepted");

            // validate
            var users = _userRepository.GetAll<Entities.User>(u => u.Email == model.Email).ToList();
            if (users.Any())
            {
                // send already registered error in email to prevent account multiplication
                SendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            var user = _mapper.Map<Entities.User>(model);

            // first registered account is an admin
            var isFirstAccount = !users.Any();
            user.RoleId = isFirstAccount ? (int) Role.Admin : (int) Role.User;

            user.CreatedOn = DateTime.UtcNow;
            user.VerificationToken = GenerateRandomToken();
            user.PasswordHash = BC.HashPassword(model.Password);

            // save user
            _userRepository.Insert(user);
            _userRepository.SaveChanges();

            // send email
            SendVerificationEmail(user, origin);
        }

        public void VerifyEmail(ValidationRequest model)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.VerificationToken == model.Token);

            if (user == null) throw new BadRequestException("Verification failed");

            user.VerifiedOn = DateTime.UtcNow;
            user.VerificationToken = null;

            _userRepository.Update(user);
            _userRepository.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Email == model.Email);

            // always return ok response to prevent email spamming
            if (user == null)
                return;

            // create reset token that expires after 1 day
            user.ResetToken = GenerateRandomToken();
            user.ResetTokenExpiresOn = DateTime.UtcNow.AddHours(24);

            _userRepository.Update(user);
            _userRepository.SaveChanges();

            // send email
            SendPasswordResetEmail(user, origin);
        }

        public void ValidateResetToken(ValidationRequest model)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u =>
                u.ResetToken == model.Token &&
                u.ResetTokenExpiresOn > DateTime.UtcNow);

            if (user == null)
                throw new BadRequestException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u =>
                u.ResetToken == model.Token &&
                u.ResetTokenExpiresOn > DateTime.UtcNow);

            if (user == null)
                throw new BadRequestException("Invalid token");

            // update password and remove reset token
            user.PasswordHash = BC.HashPassword(model.Password);
            user.PasswordResetOn = DateTime.UtcNow;
            user.ResetToken = null;
            user.ResetTokenExpiresOn = null;

            _userRepository.Update(user);
            _userRepository.SaveChanges();
        }

        public IEnumerable<User> GetUsers()
        {
            return _mapper.Map<IEnumerable<User>>(_userRepository.GetAll<Entities.User>());
        }

        public User GetUser(int id)
        {
            return _mapper.Map<User>(_userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id));
        }

        public IEnumerable<string> GetUserRefreshTokens(int id)
        {
            return _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id, $"{nameof(Entities.User.RefreshTokens)}").RefreshTokens.Select(rt => rt.Token);
        }

        public User Update(int id, UpdateUserRequest model)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new BadRequestException("User not found");

            // validate
            if (user.Email != model.Email && _userRepository.Exists<Entities.User>(u => u.Email == user.Email))
                throw new BadRequestException($"Email '{model.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                user.PasswordHash = BC.HashPassword(model.Password);

            _mapper.Map(model, user);
            user.UpdatedOn = DateTime.UtcNow;
            _userRepository.Update(user);
            _userRepository.SaveChanges();

            return _mapper.Map<User>(user);
        }

        public bool Delete(int id)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new BadRequestException("User to delete could not be found.");

            _userRepository.Delete(user);
            return _userRepository.SaveChanges();
        }

        #region helpers

        private (Entities.RefreshToken, Entities.User) GetRefreshToken(string token)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
                throw new BadRequestException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
            if (!refreshToken.IsActive)
                throw new BadRequestException("Invalid token");

            return (refreshToken, user);
        }

        private string GenerateJwtToken(Entities.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("role", user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static Entities.RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new Entities.RefreshToken
            {
                Token = GenerateRandomToken(),
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                CreatedOn = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private static string GenerateRandomToken()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void SendVerificationEmail(Entities.User user, string origin)
        {
            string message;
            if (!string.IsNullOrWhiteSpace(origin))
            {
                var verifyUrl = $"{origin}/users/verify-email?token={user.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else throw new ApplicationException("Origin url must be provided to generate the verification link!");

            _emailService.SendEmailAsync(
                user.Email,
                "Recipe Wheel Sign-up - Verify Email",
                $@"<p>Thanks for registering!</p>{message}<p>This is an automated message, do not reply to this email address!</p>"
            );
        }

        private void SendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrWhiteSpace(origin))
            {
                message = $@"<p>If you can't remember your password, please visit the <a href=""{origin}/users/forgot-password"">forgot password</a> page.</p>";
            }
            else throw new ApplicationException("Origin url must be provided to generate the verification link!");

            _emailService.SendEmailAsync(
                email,
                "Recipe Wheel Sign-up - Email Already Registered",
                $@"<p>Your email <strong>{email}</strong> is already registered.</p>{message}<p>This is an automated message, do not reply to this email address!</p>"
            );
        }

        private void SendPasswordResetEmail(Entities.User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={user.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 24 hours:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else throw new ApplicationException("Origin url must be provided to generate the verification link!");

            _emailService.SendEmailAsync(
                user.Email,
                "Recipe Wheel Sign-up - Reset Password",
                $@"{message}<p>This is an automated message, do not reply to this email address!</p>"
            );
        }

        #endregion

    }
}
