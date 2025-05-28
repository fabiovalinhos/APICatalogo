using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/teste")]
    [ApiVersion("2.0")]
    public class TesteV2Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok("Teste V2 - GET - Api Versão 2");
        }
    }
}