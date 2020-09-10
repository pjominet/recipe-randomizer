using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Web.Utils;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("auth")]
    public class AuthController : ApiController
    {
        // returns the current authenticated user (null if not logged in)
        private new User User => (User) HttpContext.Items[$"{nameof(RecipeRandomizer.Business.Models.Identity.User)}"];
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("authenticate")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> Authenticate([FromBody] AuthRequest request)
        {
            var (user, refreshToken) = await _authService.Authenticate(request, GetIpAddress());
            SetTokenCookie(refreshToken);
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var (user, newRefreshToken) = await _authService.RefreshToken(refreshToken, GetIpAddress());

            if (user == null)
                return NoContent();

            SetTokenCookie(newRefreshToken);
            return Ok(user);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeToken([FromBody] ValidationRequest request)
        {
            // accept token from request body or cookie
            var token = request.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new SimpleResponse{Message = "A refresh token is required"});

            // users can revoke their own tokens and admins can revoke any tokens
            if (!await OwnsToken(User?.Id, token) && User?.Role != Role.Admin)
                return Unauthorized(new SimpleResponse{Message = "Unauthorized"});

            await _authService.RevokeToken(token, GetIpAddress());
            return Ok(new {message = "Token revoked"});
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleResponse>> Register([FromBody] RegisterRequest request)
        {
            await _authService.Register(request, Request.Headers["origin"]);
            return Ok(new SimpleResponse{Message = "Registration successful, please check your email for verification instructions"});
        }

        [HttpPost("verify-email")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleResponse>> VerifyEmail([FromBody] ValidationRequest request)
        {
            await _authService.VerifyEmail(request);
            return Ok(new SimpleResponse{Message = "Verification successful, you can now login"});
        }

        [HttpPost("resend-email-verification")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleResponse>> ResendEmailVerificationCode([FromBody] VerificationRequest request)
        {
            await _authService.ResendEmailVerificationCode(request, Request.Headers["origin"]);
            return Ok(new SimpleResponse{Message = "New code sent, please check your email for verification instructions"});
        }

        [HttpPost("forgot-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleResponse>> ForgotPassword([FromBody] VerificationRequest request)
        {
            await _authService.ForgotPassword(request, Request.Headers["origin"]);
            return Ok(new SimpleResponse{Message = "Please check your email for password reset instructions"});
        }

        [HttpPost("validate-reset-token")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleResponse>> ValidateResetToken([FromBody] ValidationRequest request)
        {
            await _authService.ValidateResetToken(request);
            return Ok(new SimpleResponse{Message = "Token is valid"});
        }

        [HttpPost("reset-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPassword(request);
            return Ok(new SimpleResponse{Message = "Password reset successful, you can now login again"});
        }

        [HttpPost("reset-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleResponse>> ResetPassword([FromBody] ChangePasswordRequest request)
        {
            await _authService.ChangePassword(request);
            return Ok(new SimpleResponse{Message = "Password change successful"});
        }

        #region helpers

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];

            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private async Task<bool> OwnsToken(int? userId, string token)
        {
            if (!userId.HasValue)
                return false;

            var tokens = await _authService.GetUserRefreshTokens(userId.Value);
            return tokens.FirstOrDefault(t => t == token) != null;
        }

        #endregion

    }
}
