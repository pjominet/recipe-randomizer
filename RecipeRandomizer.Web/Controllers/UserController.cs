using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Web.Utils;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("users")]
    public class UserController : ApiController
    {
        // returns the current authenticated user (null if not no valid jwt token was received)
        private new User User => (User) HttpContext.Items[$"{nameof(RecipeRandomizer.Business.Models.Identity.User)}"];
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> GetUser([FromRoute] int id)
        {
            // users can get their own account and admins can get any account
            if (id != User?.Id && User?.Role != Role.Admin)
                return Unauthorized(new SimpleResponse{Message = "Unauthorized"});

            return Ok(await _userService.GetUserAsync(id));
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> UpdateUser([FromRoute] int id, [FromBody] UserUpdateRequest userUpdateRequest)
        {
            // users can update their own account and admins can update any account
            if (id != User?.Id && User?.Role != Role.Admin)
                return Unauthorized(new SimpleResponse{Message = "Unauthorized"});

            return Ok(await _userService.UpdateAsync(id, userUpdateRequest));
        }

        [Authorize(Role.Admin)]
        [HttpPut("{id:int}/role")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserRole([FromRoute] int id, [FromBody] RoleUpdateRequest roleUpdateRequest)
        {
            return Ok(await _userService.UpdateAsync(id, roleUpdateRequest));
        }

        [Authorize]
        [HttpPost("image-upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadUserAvatar([FromForm(Name = "file")] IFormFile image, [FromForm(Name = "id")] int? userId)
        {
            if (image == null || image.Length <= 0 || !userId.HasValue)
                return BadRequest("Missing information");

            if (image.Length > 2097152) // > 2MB
                return BadRequest("File is too large");

            var stream = image.OpenReadStream();
            var result = await _userService.UploadUserAvatar(stream, image.FileName, userId.Value);
            stream.Close();

            return result
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            // users can delete their own account and admins can delete any account
            if (id != User?.Id && User?.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            return await _userService.Delete(id)
                ? Ok(new SimpleResponse{Message = "Account deleted successfully"})
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize(Role.Admin)]
        [HttpPost("lock/{id:int}")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleUserLock([FromRoute] int id, [FromBody] LockRequest lockRequest)
        {
            return await _userService.ToggleUserLock(id, lockRequest)
                ? Ok(new SimpleResponse{Message = "Successfully changed user lock"})
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
