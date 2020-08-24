using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IUserService
    {
        public User Authenticate(AuthRequest model, string ipAddress);
        public User RefreshToken(string token, string ipAddress);
        public void RevokeToken(string token, string ipAddress);
        public void Register(RegisterRequest model, string origin);
        public void VerifyEmail(ValidationRequest model);
        public void ForgotPassword(ForgotPasswordRequest model, string origin);
        public void ValidateResetToken(ValidationRequest model);
        public void ResetPassword(ResetPasswordRequest model);
        public IEnumerable<User> GetUsers();
        public User GetUser(int id);
        public IEnumerable<string> GetUserRefreshTokens(int id);
        User Update(int id, UpdateUserRequest model);
        bool Delete(int id);
    }
}
