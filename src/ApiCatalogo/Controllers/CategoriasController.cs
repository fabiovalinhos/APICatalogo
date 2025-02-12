using ApiCatalogo.DTOs;
using ApiCatalogo.Filters;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ILogger<CategoriasController> logger, IUnitOfWork uof)
        {
            _logger = logger;
            _uof = uof;
        }

        //No controlador não tenho mais um try catch pois eu tenho um filtro global para pegar
        //    Exceptions (ApiExceptionFilter)

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            var categorias = _uof.CategoriaRepository.GetAll();

            if(categorias is null ){
                return NotFound("Categorias nao encontradas ... ");
            }



            return Ok(categorias.ParaDTOLista());
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            // teste de excessões
            // throw new Exception("Exceção ao retornar o produto pelo ID");

            // string [] teste = null;
            // if (teste.Length >0 ){}

            _logger.LogInformation($"========= GET categorias/id = {id} ==========");

            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogWarning($"Categoria com id = {id} não foi encontrado");
                return NotFound($"Categoria id = {id} não encontrada...");
            }

            var categoriaDTO = categoria.ParaDTO();

            return Ok(categoriaDTO);
        }


        [HttpPost]
        public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO is null)
                return BadRequest();

            var categoria = categoriaDTO.ParaCategoria();

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            _uof.Commit();

            var novaCategoriaDTO = categoriaCriada.ParaDTO();

            return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDTO.CategoriaId }, novaCategoriaDTO);
        }

        [HttpPut]
        public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDTO)
        {
            if (id != categoriaDTO.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos ...");
                return BadRequest("Invalido ...");
            }

            var categoria = categoriaDTO.ParaCategoria();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
            _uof.Commit();

            var novaCategoriaDTO = categoriaAtualizada.ParaDTO();

            return Ok(novaCategoriaDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categoria is null)
                return NotFound($"Categoria id = {id} não encontrada ...");


            var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
            _uof.Commit();

            var categDTO = categoriaExcluida.ParaDTO();
            return Ok(categDTO);
        }
    }
}