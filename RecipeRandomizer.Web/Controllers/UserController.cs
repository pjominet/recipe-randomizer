using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Web.Controllers
{
    [Authorize]
    [Route("users")]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var user = _userService.Authenticate(model, IpAddress());

            if (user == null)
                return Unauthorized(new {message = "Username or password is incorrect"});

            SetTokenCookie(user.RefreshToken);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                var id = _userService.Create(user, user.Password);
                return id != -1
                    ? CreatedAtAction(nameof(GetUser), new {id}, user)
                    : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = _userService.RefreshToken(refreshToken, IpAddress());

            if (user == null)
                return Unauthorized(new {message = "Invalid token"});

            SetTokenCookie(user.RefreshToken);

            return Ok(user);
        }

        [HttpPost("revoke-token")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new {message = "Token is required"});

            var result = _userService.RevokeToken(token, IpAddress());

            if (!result)
                return NotFound(new {message = "Token not found"});

            return Ok(new {message = "Token revoked"});
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public IActionResult GetUser([FromRoute] int id)
        {
            var user = _userService.GetUser(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}/refresh-tokens")]
        [ProducesResponseType(typeof(List<RefreshToken>), StatusCodes.Status200OK)]
        public IActionResult GetRefreshTokens(int id)
        {
            return Ok(_userService.GetUserRefreshTokens(id));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(User), StatusCodes.Status400BadRequest)]
        public IActionResult Update([FromBody] User model)
        {
            try
            {
                var result = _userService.Update(model, model.Password);
                return result
                    ? NoContent()
                    : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            var result = _userService.Delete(id);
            return result
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
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

        #endregion
    }
}
