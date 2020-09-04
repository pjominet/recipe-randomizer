using System.Collections.Generic;
using System.Threading.Tasks;
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
        // returns the current authenticated user (null if not no valid jwt token was received)
        private new User User => (User) HttpContext.Items[$"{nameof(RecipeRandomizer.Business.Models.Identity.User)}"];
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_userService.GetUsers());
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> GetUser([FromRoute] int id)
        {
            // users can get their own account and admins can get any account
            if (id != User?.Id && User?.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            return Ok(_userService.GetUser(id));
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public ActionResult<User> Update([FromRoute] int id, [FromBody] UpdateRequest updateRequest)
        {
            // users can update their own account and admins can update any account
            if (id != User?.Id && User?.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            return Ok(_userService.Update(id, updateRequest));
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
                return BadRequest("Missing information in form-data");

            Task<bool> result;
            await using (var stream = image.OpenReadStream())
            {
                result = _userService.UploadUserAvatar(stream, image.FileName, userId.Value);
            }

            return await result ? NoContent() : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete([FromRoute] int id)
        {
            // users can delete their own account and admins can delete any account
            if (id != User?.Id && User?.Role != Role.Admin)
                return Unauthorized(new {message = "Unauthorized"});

            return _userService.Delete(id)
                ? Ok(new {message = "Account deleted successfully"})
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize(Role.Admin)]
        [HttpPost("{id:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ToggleUserLock([FromRoute] int id, [FromBody] LockRequest lockRequest)
        {
            return _userService.ToggleUserLock(id, lockRequest)
                ? Ok(new {message = "Successfully changed user lock"})
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
