using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IAuthService
    {
        public Task<(User, string)> Authenticate(AuthRequest request, string ipAddress);
        public Task<(User, string)> RefreshToken(string token, string ipAddress);
        public Task RevokeToken(string token, string ipAddress);
        public Task Register(RegisterRequest request, string origin);
        public Task VerifyEmail(ValidationRequest request);
        public Task ResendEmailVerificationCode(VerificationRequest request, string origin);
        public Task ForgotPassword(VerificationRequest request, string origin);
        public Task ValidateResetToken(ValidationRequest request);
        public Task ResetPassword(ResetPasswordRequest request);
        public Task ChangePassword(ChangePasswordRequest request);
        public Task<IEnumerable<string>> GetUserRefreshTokens(int id);
    }
}
