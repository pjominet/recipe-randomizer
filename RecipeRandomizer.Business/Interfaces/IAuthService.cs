using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IAuthService
    {
        public (User, string) Authenticate(AuthRequest request, string ipAddress);
        public (User, string) RefreshToken(string token, string ipAddress);
        public void RevokeToken(string token, string ipAddress);
        public void Register(RegisterRequest request, string origin);
        public void VerifyEmail(ValidationRequest request);
        public void ResendEmailVerificationCode(VerificationRequest request, string origin);
        public void ForgotPassword(VerificationRequest request, string origin);
        public void ValidateResetToken(ValidationRequest request);
        public void ResetPassword(ResetPasswordRequest request);
        public void ChangePassword(ChangePasswordRequest request);
        public IEnumerable<string> GetUserRefreshTokens(int id);
    }
}
