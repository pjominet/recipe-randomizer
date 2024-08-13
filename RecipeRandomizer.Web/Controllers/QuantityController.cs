using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Nomenclature;

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

        [HttpGet("units")]
        [ProducesResponseType(typeof(IEnumerable<QuantityUnit>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<QuantityUnit>>> GetQuantityUnits()
        {
            return Ok(await _quantityService.GetQuantityUnitsAsync());
        }
    }
}
