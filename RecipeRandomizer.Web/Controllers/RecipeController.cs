using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using System.Linq;

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
        public IActionResult GetRecipes([FromQuery(Name = "tag")] int[] tagIds, [FromQuery(Name = "user")] int? userId = null)
        {
            if (tagIds.Any() && !userId.HasValue)
                return Ok(_recipeService.GetRecipesFromTags(tagIds));
            if (!tagIds.Any() && userId.HasValue)
                return Ok(_recipeService.GetRecipesFromUser(userId.Value));
            if (tagIds.Any() && userId.HasValue)
                return Ok(_recipeService.GetRecipesFromUserAndTags(userId.Value, tagIds));

            return Ok(_recipeService.GetRecipes());
        }

        [HttpGet("deleted")]
        [ProducesResponseType(typeof(List<Recipe>), StatusCodes.Status200OK)]
        public IActionResult GetDeletedRecipes()
        {
            return Ok(_recipeService.GetDeletedRecipes());
        }

        [HttpGet("{id}")]
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
        public IActionResult GetRandomRecipe()
        {
            var recipe = _recipeService.GetRandomRecipe();
            return recipe != null
                ? Ok(recipe)
                : (IActionResult) NotFound();
        }


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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRecipe([FromRoute] int id, [FromQuery(Name = "hard")] bool hard = false)
        {
            return _recipeService.DeleteRecipe(id, hard)
                ? NoContent()
                : StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPut]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EditRecipe([FromBody] Recipe recipe)
        {
            return _recipeService.UpdateRecipe(recipe)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
