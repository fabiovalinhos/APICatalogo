using ApiCatalogo.Context;
using ApiCatalogo.Filters;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repository;
        private readonly ILogger _logger;

        public CategoriasController(ICategoriaRepository repository, ILogger<CategoriasController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        //No controlador não tenho mais um try catch pois eu tenho um filtro global para pegar
        //    Exceptions (ApiExceptionFilter)

        // [HttpGet("produtos")]
        // public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        // {
        //     _logger.LogInformation("========= GET categorias/produtos ==========");

        //     return _context.Categorias.Include(p => p.Produtos).ToList();
        // }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            var categorias = _repository.GetCategorias();

            return Ok(categorias);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            // teste de excessões
            // throw new Exception("Exceção ao retornar o produto pelo ID");

            // string [] teste = null;
            // if (teste.Length >0 ){}

            _logger.LogInformation($"========= GET categorias/id = {id} ==========");

            var categoria = _repository.GetCategoria(id);

            if (categoria is null)
            {
                _logger.LogWarning($"Categoria com id = {id} não foi encontrado");
                return NotFound("Categoria não encontrada...");
            }

            return Ok(categoria);
        }


        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
                return BadRequest();

            var categoriaCriada = _repository.Create(categoria);

            return new CreatedAtRouteResult("ObterCategoria",
            new { id = categoriaCriada.CategoriaId }, categoria);
        }

        [HttpPut]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos ...");
                return BadRequest();
            }

            _repository.Update(categoria);
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Categoria> Delete(int id)
        {
            var categoria = _repository.GetCategoria(id);

            if (categoria is null)
                return NotFound($"Categoria id = {id} não encontrada ...");


            var categoriaExcluida = _repository.Delete(id);
            return Ok(categoriaExcluida);
        }
    }
}