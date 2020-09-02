using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Web.Utils;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("auth")]
    public class AuthController : ApiController
    {
        // returns the current authenticated user (null if not logged in)
        private new User User => (User) HttpContext.Items["User"];
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("authenticate")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> Authenticate([FromBody] AuthRequest request)
        {
            var user = _authService.Authenticate(request, IpAddress());
            SetTokenCookie(user.RefreshToken);
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
                return NoContent();
            var user = _authService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(user.RefreshToken);
            return Ok(user);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult RevokeToken([FromBody] ValidationRequest request)
        {
            // accept token from request body or cookie
            var token = request.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new {message = "Token is required"});

            // users can revoke their own tokens and admins can revoke any tokens
            if (!OwnsToken(User.Id, token) && User.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            _authService.RevokeToken(token, IpAddress());
            return Ok(new {message = "Token revoked"});
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            _authService.Register(request, Request.Headers["origin"]);
            return Ok(new {message = "Registration successful, please check your email for verification instructions"});
        }

        [HttpPost("verify-email")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult VerifyEmail([FromBody] ValidationRequest request)
        {
            _authService.VerifyEmail(request);
            return Ok(new {message = "Verification successful, you can now login"});
        }

        [HttpPost("resend-email-verification")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ResendEmailVerificationCode([FromBody] VerificationRequest request)
        {
            _authService.ResendEmailVerificationCode(request, Request.Headers["origin"]);
            return Ok(new {message = "New code sent, please check your email for verification instructions"});
        }

        [HttpPost("forgot-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ForgotPassword([FromBody] VerificationRequest request)
        {
            _authService.ForgotPassword(request, Request.Headers["origin"]);
            return Ok(new {message = "Please check your email for password reset instructions"});
        }

        [HttpPost("validate-reset-token")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ValidateResetToken([FromBody] ValidationRequest request)
        {
            _authService.ValidateResetToken(request);
            return Ok(new {message = "Token is valid"});
        }

        [HttpPost("reset-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            _authService.ResetPassword(request);
            return Ok(new {message = "Password reset successful, you can now login again"});
        }

        [HttpPost("reset-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ResetPassword([FromBody] ChangePasswordRequest request)
        {
            _authService.ChangePassword(request);
            return Ok(new {message = "Password change successful"});
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

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];

            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private bool OwnsToken(int userId, string token)
        {
            var tokens = _authService.GetUserRefreshTokens(userId);
            return tokens.FirstOrDefault(t => t == token) != null;
        }

        #endregion

    }
}
