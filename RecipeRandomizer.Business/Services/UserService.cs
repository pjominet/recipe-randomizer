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
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Business.Services
{
    public class UserService : IUserService
    {

        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly string _jwtSecret;

        public UserService(RRContext context, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
            _jwtSecret = configuration.GetValue<string>("JWTSecret");
        }

        public User Authenticate(AuthRequest model, string ipAddress)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(x => x.Email == model.Email);

            // return null if user not found
            if (user == null) return null;

            // check if password is correct
            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // save refresh token
            user.RefreshTokens.Add(refreshToken);
            _userRepository.Update(user);
            _userRepository.SaveChanges();

            return new User(user, jwtToken, refreshToken.Token);
        }

        public User RefreshToken(string token, string ipAddress)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.RefreshTokens.Any(t => t.Token == token));

            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _userRepository.Update(user);
            _userRepository.SaveChanges();

            // generate new jwt
            var jwtToken = GenerateJwtToken(user);

            return new User(user, jwtToken, newRefreshToken.Token);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _userRepository.Update(user);
            _userRepository.SaveChanges();

            return true;
        }

        public IEnumerable<User> GetUsers()
        {
            return _mapper.Map<IEnumerable<User>>(_userRepository.GetAll<Entities.User>());
        }

        public User GetUser(int id)
        {
            return _mapper.Map<User>(_userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id));
        }

        public IEnumerable<RefreshToken> GetUserRefreshTokens(int id)
        {
            return _mapper.Map<IEnumerable<RefreshToken>>(_userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id, $"{nameof(Entities.User.RefreshTokens)}").RefreshTokens);
        }

        public int Create(User model, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException("Password is required");

            if (_userRepository.Exists<Entities.User>(u => u.Email == model.Email))
                throw new ApplicationException("Username \"" + model.Email + "\" is already taken");

            var user = _mapper.Map<Entities.User>(model);
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userRepository.Insert(user);

            return _userRepository.SaveChanges() ? user.Id : -1;;
        }

        public bool Update(User model, string password = null)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == model.Id);

            if (user == null)
                throw new ApplicationException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
            {
                // throw error if the new username is already taken
                if (_userRepository.Exists<Entities.User>(u => u.Email == user.Email))
                    throw new ApplicationException("Username " + model.Email + " is already taken");

                user.Email = model.Email;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                user.LastName = model.LastName;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _userRepository.Update(model);
            return _userRepository.SaveChanges();
        }

        public bool Delete(int id)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new ApplicationException("User to delete could not be found.");

            _userRepository.Delete(user);
            return _userRepository.SaveChanges();
        }

        #region helpers

        private string GenerateJwtToken(Entities.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret)), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private static Entities.RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return new Entities.RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only.", nameof(password));

            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, IReadOnlyList<byte> storedHash, byte[] storedSalt)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only.", nameof(password));

            if (storedHash.Count != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));

            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));

            using var hmac = new HMACSHA512(storedSalt);

            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password)).Where((t, i) => t != storedHash[i]).Any();
        }

        #endregion
    }
}
