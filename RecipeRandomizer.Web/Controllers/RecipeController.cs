using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using System.Linq;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Web.Utils;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("recipes")]
    public class RecipeController : ApiController
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Recipe>), StatusCodes.Status200OK)]
        public IActionResult GetRecipes([FromQuery(Name = "tag")] int[] tagIds)
        {
            return Ok(tagIds.Any()
                ? _recipeService.GetRecipesFromTags(tagIds)
                : _recipeService.GetRecipes());
        }

        [Authorize]
        [HttpGet("created/{id:int}")]
        [ProducesResponseType(typeof(List<Recipe>), StatusCodes.Status200OK)]
        public IActionResult GetRecipesForUser([FromRoute(Name = "id")] int id)
        {
            return Ok(_recipeService.GetRecipesForUser(id));
        }

        [Authorize]
        [HttpGet("liked/{id:int}")]
        [ProducesResponseType(typeof(List<Recipe>), StatusCodes.Status200OK)]
        public IActionResult GetLikedRecipesForUser([FromRoute(Name = "id")] int id)
        {
            return Ok(_recipeService.GetLikedRecipesForUser(id));
        }

        [HttpGet("deleted")]
        [ProducesResponseType(typeof(List<Recipe>), StatusCodes.Status200OK)]
        public IActionResult GetDeletedRecipes()
        {
            return Ok(_recipeService.GetDeletedRecipes());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRecipe([FromRoute] int id)
        {
            var recipe = _recipeService.GetRecipe(id);
            return recipe != null
                ? Ok(recipe)
                : (IActionResult) NotFound();
        }

        [HttpGet("random")]
        [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRandomRecipe([FromQuery(Name = "tag")] int[] tagIds)
        {
            var recipeId = _recipeService.GetRandomRecipe(tagIds);
            return recipeId.HasValue
                ? Ok(recipeId.Value)
                : (IActionResult) NotFound();
        }

        [Authorize(Role.Admin)]
        [HttpGet("orphans")]
        [ProducesResponseType(typeof(List<Recipe>), StatusCodes.Status200OK)]
        public IActionResult GetOrphanRecipes()
        {
            return Ok(_recipeService.GetOrphanRecipes());
        }

        [Authorize(Role.Admin)]
        [HttpPost("orphans")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(List<Recipe>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AttributeRecipe([FromBody] AttributionRequest request)
        {
            return _recipeService.AttributeRecipe(request)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddRecipe([FromBody] Recipe recipe)
        {
            var id = _recipeService.CreateRecipe(recipe);
            return id != -1
                ? CreatedAtAction(nameof(GetRecipe), new {id}, recipe)
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPost("image-upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UploadRecipeImage([FromForm(Name = "file")] IFormFile image, [FromForm(Name = "id")] int? recipeId)
        {
            if (image == null || image.Length <= 0 || !recipeId.HasValue)
                return BadRequest("Missing information");

            if (image.Length > 2097152) // > 2MB
                return BadRequest("File is too large");

            var stream = image.OpenReadStream();
            var result = _recipeService.UploadRecipeImage(stream, image.FileName, recipeId.Value);
            stream.Close();

            return result
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRecipe([FromRoute] int id, [FromQuery(Name = "hard")] bool hard = false)
        {
            return _recipeService.DeleteRecipe(id, hard)
                ? NoContent()
                : StatusCode(StatusCodes.Status404NotFound);
        }

        [Authorize]
        [HttpGet("restore/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RestoreDeletedRecipe([FromRoute] int id)
        {
            var restoredRecipe = _recipeService.RestoreDeletedRecipe(id);

            return restoredRecipe != null
                ? Ok(restoredRecipe)
                : (IActionResult) StatusCode(StatusCodes.Status404NotFound);
        }

        [Authorize]
        [HttpGet("{recipeId:int}/liked-by/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ToggleRecipeLike([FromRoute(Name = "recipeId")] int recipeId, [FromRoute(Name = "userId")] int userId, [FromQuery(Name = "like")] bool like = false)
        {
            return _recipeService.ToggleRecipeLike(recipeId, userId, like)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPut]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EditRecipe([FromBody] Recipe recipe)
        {
            return _recipeService.UpdateRecipe(recipe)
                ? Ok(recipe.Id)
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
