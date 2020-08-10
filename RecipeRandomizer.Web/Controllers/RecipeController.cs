using System.Collections.Generic;
using System.Data;
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
        [ProducesResponseType(typeof(List<Rule>), StatusCodes.Status200OK)]
        public IActionResult GetRecipes([FromQuery(Name = "tag")] int[] tagIds)
        {
            return tagIds.Any()
                ? Ok(_recipeService.GetRecipesFromTags(tagIds))
                : Ok(_recipeService.GetRecipes());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Rule), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRecipe([FromRoute] int id)
        {
            var tag = _recipeService.GetRecipe(id);
            return tag != null
                ? Ok(tag)
                : (IActionResult) NotFound();
        }

        [HttpGet("random")]
        [ProducesResponseType(typeof(Rule), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRandomRecipe([FromRoute] int id)
        {
            var tag = _recipeService.GetRecipe(id);
            return tag != null
                ? Ok(tag)
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

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EditRecipe([FromRoute] int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id)
                return BadRequest();

            return _recipeService.UpdateRecipe(recipe)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
