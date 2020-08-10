using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("quantities")]
    public class QuantityController : ApiController
    {
        private readonly IQuantityService _quantityService;

        public QuantityController(IQuantityService quantityService)
        {
            _quantityService = quantityService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetQuantities()
        {
            return Ok(_quantityService.GetQuantities());
        }
    }
}
