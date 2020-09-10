using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
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
        [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes([FromQuery(Name = "tag")] int[] tagIds)
        {
            return Ok(await _recipeService.GetRecipesAsync(tagIds));
        }

        [HttpGet("published-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetPublishedRecipeCount()
        {
            return Ok(await _recipeService.GetPublishedRecipeCountAsync());
        }

        [Authorize]
        [HttpGet("created/{id:int}")]
        [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesForUser([FromRoute(Name = "id")] int id)
        {
            return Ok(await _recipeService.GetRecipesForUserAsync(id));
        }

        [Authorize]
        [HttpGet("liked/{id:int}")]
        [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetLikedRecipesForUser([FromRoute(Name = "id")] int id)
        {
            return Ok(await _recipeService.GetLikedRecipesForUserAsync(id));
        }

        [HttpGet("deleted")]
        [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetDeletedRecipes()
        {
            return Ok(await _recipeService.GetDeletedRecipesAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Recipe>> GetRecipe([FromRoute] int id)
        {
            var recipe = await _recipeService.GetRecipeAsync(id);
            return recipe != null
                ? Ok(recipe)
                : (ActionResult) NotFound();
        }

        [HttpGet("random")]
        [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Recipe>> GetRandomRecipe([FromQuery(Name = "tag")] int[] tagIds)
        {
            var recipeId = await _recipeService.GetRandomRecipeIdAsync(tagIds);
            return recipeId.HasValue
                ? Ok(recipeId.Value)
                : (ActionResult) NotFound();
        }

        [Authorize(Role.Admin)]
        [HttpGet("abandoned")]
        [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAbandonedRecipes()
        {
            return Ok(await _recipeService.GetAbandonedRecipesAsync());
        }

        [Authorize(Role.Admin)]
        [HttpPost("abandoned")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Recipe>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Recipe>>> AttributeRecipe([FromBody] AttributionRequest request)
        {
            return await _recipeService.AttributeRecipeAsync(request)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Recipe), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Recipe>> AddRecipe([FromBody] Recipe recipe)
        {
            var newRecipe = await _recipeService.CreateRecipe(recipe);
            return newRecipe != null
                ? CreatedAtAction(nameof(GetRecipe), new {id = newRecipe.Id}, newRecipe)
                : (ActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPost("image-upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadRecipeImage([FromForm(Name = "file")] IFormFile image, [FromForm(Name = "id")] int? recipeId)
        {
            if (image == null || image.Length <= 0 || !recipeId.HasValue)
                return BadRequest("Missing information");

            if (image.Length > 2097152) // > 2MB
                return BadRequest("File is too large");

            var stream = image.OpenReadStream();
            var result = await _recipeService.UploadRecipeImage(stream, image.FileName, recipeId.Value);
            stream.Close();

            return result
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecipe([FromRoute] int id, [FromQuery(Name = "hard")] bool hard = false)
        {
            return await _recipeService.DeleteRecipe(id, hard)
                ? NoContent()
                : (IActionResult) NotFound();
        }

        [Authorize]
        [HttpGet("restore/{id:int}")]
        [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Recipe>> RestoreDeletedRecipe([FromRoute] int id)
        {
            var restoredRecipe = await _recipeService.RestoreDeletedRecipe(id);

            return restoredRecipe != null
                ? Ok(restoredRecipe)
                : (ActionResult) NotFound();
        }

        [Authorize]
        [HttpPost("like/{recipeId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleRecipeLike([FromRoute(Name = "recipeId")] int recipeId, [FromBody] LikeRequest request)
        {
            return await _recipeService.ToggleRecipeLike(recipeId, request)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPut]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditRecipe([FromBody] Recipe recipe)
        {
            return await _recipeService.UpdateRecipe(recipe)
                ? Ok(recipe.Id)
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
