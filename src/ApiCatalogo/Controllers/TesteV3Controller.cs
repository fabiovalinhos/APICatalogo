using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;


namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/teste")]
    [ApiVersion("3.0")]
    [ApiVersion("4.0")]
    public class TesteV3Controller : ControllerBase
    {

    [MapToApiVersion("3.0")]
    [HttpGet]
        public IActionResult GetVersion3(){
            return  Ok("Teste V3 - GET - Api Versão 3");
        }

        [MapToApiVersion("4.0")]
    [HttpGet]
        public IActionResult GetVersion4(){
            return Ok("Teste V4 - GET - Api Versão 4");
        }
    }
}