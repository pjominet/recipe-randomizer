using System.Collections.Generic;
using System.Threading.Tasks;
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
        [ProducesResponseType(typeof(IEnumerable<Tag>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            return Ok(await _tagService.GetTags());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Tag), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Task>> GetTag([FromRoute] int id)
        {
            var tag = await _tagService.GetTag(id);
            return tag != null
                ? Ok(tag)
                : (ActionResult) NotFound();
        }


        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(Tag),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Tag>> AddTag([FromBody] Tag tag)
        {
            var newTag = await _tagService.CreateTag(tag);
            return newTag != null
                ? CreatedAtAction(nameof(GetTag), new {id = newTag.Id}, newTag)
                : (ActionResult) StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTag([FromRoute] int id)
        {
            return await _tagService.DeleteTag(id)
                ? NoContent()
                : StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditTag([FromRoute] int id, [FromBody] Tag tag)
        {
            if (id != tag.Id)
                return BadRequest();

            return await _tagService.UpdateTag(tag)
                ? NoContent()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<TagCategory>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TagCategory>>> GetTagCategories()
        {
            return Ok(await _tagService.GetTagCategories());
        }

        [HttpGet("categories/{id}")]
        [ProducesResponseType(typeof(TagCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TagCategory>> GetTagCategory([FromRoute] int id)
        {
            var tagCategory = await _tagService.GetTagCategory(id);
            return tagCategory != null
                ? Ok(tagCategory)
                : (ActionResult) NotFound();
        }
    }
}
