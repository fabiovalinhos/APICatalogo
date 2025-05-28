using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/teste")]
    [ApiVersion("1.0", Deprecated = true)]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok("Teste V1 - GET - Apiversão 1");
        }
    }
}