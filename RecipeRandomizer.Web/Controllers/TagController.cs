using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("tags")]
    public class TagController : ApiController
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Rule>), StatusCodes.Status200OK)]
        public IActionResult GetTags()
        {
            return Ok(_tagService.GetTags());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Rule), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTag([FromRoute] int id)
        {
            var tag = _tagService.GetTag(id);
            return tag != null
                ? Ok(tag)
                : (IActionResult) NotFound();
        }


        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddTag([FromBody] Tag tag)
        {
            var id = _tagService.CreateTag(tag);
            return id != -1
                ? CreatedAtAction(nameof(GetTag), new {id}, tag)
                : (IActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteTag([FromRoute] int id)
        {
            return _tagService.DeleteTag(id)
                ? NoContent()
                : StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EditTag([FromRoute] int id, [FromBody] Tag tag)
        {
            if (id != tag.Id)
                return BadRequest();

            return _tagService.UpdateTag(tag)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(List<Rule>), StatusCodes.Status200OK)]
        public IActionResult GetTagCategories()
        {
            return Ok(_tagService.GetTagCategories());
        }

        [HttpGet("categories/{id}")]
        [ProducesResponseType(typeof(Rule), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTagCategory([FromRoute] int id)
        {
            var tagCategory = _tagService.GetTagCategory(id);
            return tagCategory != null
                ? Ok(tagCategory)
                : (IActionResult) NotFound();
        }
    }
}
