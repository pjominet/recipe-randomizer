using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace RecipeRandomizer.Web.Controllers
{
    [Route("")]
    public class InfoController : ApiController
    {
        private IConfiguration Configuration { get; }
        private readonly string _version;

        public InfoController(IConfiguration configuration)
        {
            Configuration = configuration;
            _version = Configuration.GetValue<string>("Version");
        }

        [HttpGet]
        [HttpGet("info")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetApiInfo()
        {
            return Ok("Recipe Repository API is currently running @" + _version);
        }

        [HttpGet("version")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetApiVersion()
        {
            return Ok(_version);
        }

        [HttpGet("love")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetLove()
        {
            return Ok("Developed with ❤️ by Pat00");
        }
    }
}
