using System.Collections.Generic;
using System.IO;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IAuthService
    {
        public User Authenticate(AuthRequest request, string ipAddress);
        public User RefreshToken(string token, string ipAddress);
        public void RevokeToken(string token, string ipAddress);
        public void Register(RegisterRequest request, string origin);
        public void VerifyEmail(ValidationRequest request);
        public void ResendEmailVerificationCode(VerificationRequest request, string origin);
        public void ForgotPassword(VerificationRequest request, string origin);
        public void ValidateResetToken(ValidationRequest request);
        public void ResetPassword(ResetPasswordRequest request);
        public void ChangePassword(ChangePasswordRequest request);
        public IEnumerable<User> GetUsers();
        public User GetUser(int id);
        public IEnumerable<string> GetUserRefreshTokens(int id);
        User Update(int id, UpdateUserRequest request);
        public bool UploadUserAvatar(Stream imageStream, int id);
        bool Delete(int id);
    }
}
