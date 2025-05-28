using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/teste")]
    [ApiVersion("1.0")]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok("Teste V1 - GET - Apiversão 1");
        }
    }
}