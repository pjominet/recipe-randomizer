using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Web.Utils;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("users")]
    public class UserController : ApiController
    {
        // returns the current authenticated user (null if not logged in)
        private new User User => (User) HttpContext.Items["User"];
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> Authenticate([FromBody] AuthRequest model)
        {
            var user = _userService.Authenticate(model, IpAddress());
            SetTokenCookie(user.RefreshToken);
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = _userService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(user.RefreshToken);
            return Ok(user);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult RevokeToken([FromBody] ValidationRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new {message = "Token is required"});

            // users can revoke their own tokens and admins can revoke any tokens
            if (!OwnsToken(User.Id, token) && User.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            _userService.RevokeToken(token, IpAddress());
            return Ok(new {message = "Token revoked"});
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult Register([FromBody] RegisterRequest model)
        {
            _userService.Register(model, Request.Headers["origin"]);
            return Ok(new {message = "Registration successful, please check your email for verification instructions"});
        }

        [HttpPost("verify-email")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult VerifyEmail([FromBody] ValidationRequest model)
        {
            _userService.VerifyEmail(model);
            return Ok(new {message = "Verification successful, you can now login"});
        }

        [HttpPost("forgot-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            _userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new {message = "Please check your email for password reset instructions"});
        }

        [HttpPost("validate-reset-token")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ValidateResetToken([FromBody] ValidationRequest model)
        {
            _userService.ValidateResetToken(model);
            return Ok(new {message = "Token is valid"});
        }

        [HttpPost("reset-password")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest model)
        {
            _userService.ResetPassword(model);
            return Ok(new {message = "Password reset successful, you can now login"});
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> GetUser([FromRoute] int id)
        {
            // users can get their own account and admins can get any account
            if (id != User.Id && User.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            var user = _userService.GetUser(id);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> Update([FromRoute] int id, [FromBody] UpdateUserRequest model)
        {
            // users can update their own account and admins can update any account
            if (id != User.Id && User.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            // only admins can update role
            if (User.Role != Role.Admin)
                model.Role = null;

            var user = _userService.Update(id, model);
            return Ok(user);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult Delete(int id)
        {
            // users can delete their own account and admins can delete any account
            if (id != User.Id && User.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            _userService.Delete(id);
            return Ok(new {message = "Account deleted successfully"});
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
            var tokens = _userService.GetUserRefreshTokens(userId);
            return tokens.FirstOrDefault(t => t == token) != null;
        }

        #endregion

    }
}
