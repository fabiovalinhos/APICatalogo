using ApiCatalogo.DTOs;
using ApiCatalogo.Filters;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixedwindow")]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ILogger<CategoriasController> logger, IUnitOfWork uof)
        {
            _logger = logger;
            _uof = uof;
        }

        // No controlador não tenho mais um try catch pois eu tenho um filtro global para pegar
        // Exceptions (ApiExceptionFilter)

        /// <summary>
        /// Obtenho uma lista de objetos Categoria
        /// </summary>
        /// <returns>Uma lista de objetos Categoria</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        // [Authorize]
        [DisableRateLimiting]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            var categorias = await _uof.CategoriaRepository.GetAllAsync(null);

            if (categorias is null)
            {
                return NotFound("Categorias nao encontradas ... ");
            }

            return Ok(categorias.ParaDTOListaMapper());
        }


        /// <summary>
        /// Obtém uma categoria pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objeto Categoria</returns>
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            // teste de excessões
            // throw new Exception("Exceção ao retornar o produto pelo ID");

            // string [] teste = null;
            // if (teste.Length >0 ){}

            _logger.LogInformation($"========= GET categorias/id = {id} ==========");

            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogWarning($"Categoria com id = {id} não foi encontrado");
                return NotFound($"Categoria id = {id} não encontrada...");
            }

            var categoriaDTO = categoria.ParaDTOMapper();

            return Ok(categoriaDTO);
        }


        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery]
    CategoriasParameters categoriasParameters)
        {
            //categorias seria a própria lista com propriedades
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);
            return ObterCategorias(categorias);
        }


        [HttpGet("filter/nome/paginacao")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas([FromQuery]
    CategoriasFiltroNome categoriasFiltroNome)
        {
            var categoriasFiltradas = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltroNome);

            return ObterCategorias(categoriasFiltradas);
        }


        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(PagedList<Models.Categoria> categorias)
        {
            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPreview
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ParaDTOListaMapper();

            return Ok(categoriasDto);
        }

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request
        /// 
        /// POST /api/categorias
        /// {
        /// "CategoriaId": 1,
        ///     "nome": "Categoria Teste",
        ///     "imageUrl": "https://example.com/image.jpg"
        /// }
        /// </remarks>
        /// <param name="categoriaDTO">Objeto Categoria</param>
        /// <returns>O objeto Categoria incluído</returns>
        /// <remarks>Retorna o objeto Categoria incluído</remarks>
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO is null)
                return BadRequest();

            var categoria = categoriaDTO.ParaCategoriaMapper();

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            await _uof.CommitAsync();

            var novaCategoriaDTO = categoriaCriada.ParaDTOMapper();

            return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDTO.CategoriaId }, novaCategoriaDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDTO)
        {
            if (id != categoriaDTO.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos ...");
                return BadRequest("Invalido ...");
            }

            var categoria = categoriaDTO.ParaCategoriaMapper();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
            await _uof.CommitAsync();

            var novaCategoriaDTO = categoriaAtualizada.ParaDTOMapper();

            return Ok(novaCategoriaDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria is null)
                return NotFound($"Categoria id = {id} não encontrada ...");


            var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
            await _uof.CommitAsync();

            var categDTO = categoriaExcluida.ParaDTOMapper();
            return Ok(categDTO);
        }
    }
}