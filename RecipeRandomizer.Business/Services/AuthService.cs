using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Business.Utils.Exceptions;
using RecipeRandomizer.Business.Utils.Settings;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities.Identity;
using BC = BCrypt.Net.BCrypt;

namespace RecipeRandomizer.Business.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;

        public AuthService(RRContext context, IMapper mapper, IOptions<AppSettings> appSettings, IEmailService emailService)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }

        public User Authenticate(AuthRequest request, string ipAddress)
        {
            string[] includes =
            {
                $"{nameof(Entities.User.Role)}",
                $"{nameof(Entities.User.RefreshTokens)}"
            };
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Email == request.Email, includes);

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
                throw new BadRequestException("Email or password is incorrect");

            if(!user.IsVerified)
                throw new BadRequestException("Email has not been verified");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(user);

            // check if there is already an active refresh token and use that instead of generating a new one
            var activeRefreshToken = user.RefreshTokens.SingleOrDefault(r => r.IsActive && r.ExpiresOn >= DateTime.UtcNow);
            var refreshToken = activeRefreshToken ?? GenerateRefreshToken(ipAddress);

            // save refresh token
            user.RefreshTokens.Add(refreshToken);
            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            var authenticatedUser = _mapper.Map<User>(user);
            authenticatedUser.JwtToken = jwtToken;
            authenticatedUser.RefreshToken = refreshToken.Token;

            return authenticatedUser;
        }

        public User RefreshToken(string token, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var (refreshToken, user) = GetRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.RevokedOn = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            var jwtToken = GenerateJwtToken(user);
            var authenticatedUser = _mapper.Map<User>(user);
            authenticatedUser.JwtToken = jwtToken;
            authenticatedUser.RefreshToken = newRefreshToken.Token;

            return authenticatedUser;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, user) = GetRefreshToken(token);

            // revoke token and save
            refreshToken.RevokedOn = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");
        }

        public void Register(RegisterRequest request, string origin)
        {
            if (!request.HasAcceptedTerms)
                throw new BadRequestException("Terms and services have not been accepted");

            // check if user already exists
            if (_userRepository.Exists<Entities.User>(u => u.Email == request.Email))
            {
                // send already registered error in email to prevent account multiplication
                SendAlreadyRegisteredEmail(request.Email, origin);
                return;
            }

            var user = _mapper.Map<Entities.User>(request);

            // first registered account is an admin
            user.RoleId = !_userRepository.HasUsers() ? (int) Role.Admin : (int) Role.User;

            user.CreatedOn = DateTime.UtcNow;
            user.VerificationToken = GenerateRandomToken();
            user.PasswordHash = BC.HashPassword(request.Password);

            _userRepository.Insert(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            SendVerificationEmail(user, origin);
        }

        public void VerifyEmail(ValidationRequest request)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.VerificationToken == request.Token);

            if (user == null) throw new BadRequestException("Verification failed");

            user.VerifiedOn = DateTime.UtcNow;
            user.VerificationToken = null;

            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");
        }

        public void ResendEmailVerificationCode(VerificationRequest request, string origin)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Email == request.Email);

            if(user == null)
                throw new BadRequestException("No user matches the given email address");

            // generate new verification token
            user.VerificationToken = GenerateRandomToken();

            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            SendVerificationEmail(user, origin);
        }

        public void ForgotPassword(VerificationRequest request, string origin)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Email == request.Email);

            // always return ok response to prevent email spamming
            if (user == null)
                return;

            // create reset token that expires after 1 day
            user.ResetToken = GenerateRandomToken();
            user.ResetTokenExpiresOn = DateTime.UtcNow.AddHours(24);

            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            // send email
            SendPasswordResetEmail(user, origin);
        }

        public void ValidateResetToken(ValidationRequest request)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u =>
                u.ResetToken == request.Token &&
                u.ResetTokenExpiresOn > DateTime.UtcNow);

            if (user == null)
                throw new BadRequestException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest request)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u =>
                u.ResetToken == request.Token &&
                u.ResetTokenExpiresOn > DateTime.UtcNow);

            if (user == null)
                throw new BadRequestException("Invalid token");

            // update password and remove reset token
            user.PasswordHash = BC.HashPassword(request.Password);
            user.PasswordResetOn = DateTime.UtcNow;
            user.ResetToken = null;
            user.ResetTokenExpiresOn = null;

            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");
        }

        public void ChangePassword(ChangePasswordRequest request)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == request.Id);

            if (user == null)
                throw new BadRequestException("User could not be found");

            if(!BC.Verify(request.Password, user.PasswordHash))
                throw new BadRequestException("Current password is not correct");

            // update password
            user.PasswordHash = BC.HashPassword(request.NewPassword);
            user.PasswordResetOn = DateTime.UtcNow;

            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");
        }

        public IEnumerable<string> GetUserRefreshTokens(int id)
        {
            return _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id, $"{nameof(Entities.User.RefreshTokens)}").RefreshTokens.Select(rt => rt.Token);
        }

        #region helpers

        private (Entities.RefreshToken, Entities.User) GetRefreshToken(string token)
        {
            var refreshToken = _userRepository.GetFirstOrDefault<Entities.RefreshToken>(rt => rt.Token == token);
            if (refreshToken == null)
                throw new ApplicationException("No refresh-token found");

            string[] includes =
            {
                $"{nameof(Entities.User.Role)}",
                $"{nameof(Entities.User.RefreshTokens)}"
            };
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == refreshToken.UserId, includes);
            if (user == null)
                throw new BadRequestException("Invalid token: Token does not match any known user.");

            if (!refreshToken.IsActive)
                throw new BadRequestException("Invalid token: Token is not active anymore.");

            return (refreshToken, user);
        }

        private string GenerateJwtToken(Entities.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("role", user.Role.Label)
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
                var verifyUrl = $"{origin}/auth/verify-email?token={user.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else throw new ApplicationException("Origin url must be provided to generate the verification link!");

            _emailService.SendEmailAsync(
                user.Email,
                "Recipe Wheel Sign-up: Verify Email",
                $@"<p>Thank you for registering with <strong>Recipe Wheel</strong>!</p>{message}<p>This is an automated message, do not reply to this email address!</p>"
            );
        }

        private void SendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrWhiteSpace(origin))
            {
                message = $@"<p>If you can't remember your password, please visit the <a href=""{origin}/forgot-password"">forgot password</a> page.</p>";
            }
            else throw new ApplicationException("Origin url must be provided to generate the verification link!");

            _emailService.SendEmailAsync(
                email,
                "Recipe Wheel Sign-up: Email Already Registered",
                $@"<p>Your email <strong>{email}</strong> is already registered.</p>{message}<p>This is an automated message, do not reply to this email address!</p>"
            );
        }

        private void SendPasswordResetEmail(Entities.User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/auth/reset-password?token={user.ResetToken}";
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
