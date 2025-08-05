using ApiCatalogo.DTOs;
using ApiCatalogo.Filters;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixedwindow")]
    [Produces("application/json")] // Todos os métodos deste controller produzem JSON
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger<CategoriasController> _logger;

        private readonly IMemoryCache _cache;

        private const string CacheCategoriasKey = "CacheCategorias";

        public CategoriasController(ILogger<CategoriasController> logger, IUnitOfWork uof,
            IMemoryCache cache)
        {
            _logger = logger;
            _uof = uof;
            _cache = cache;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            if (_cache.TryGetValue(CacheCategoriasKey, out IEnumerable<CategoriaDTO> categoriasDTO))
            {
                return Ok(categoriasDTO);
            }

            var categorias = await _uof.CategoriaRepository.GetAllAsync(null);
            if (categorias is null || !categorias.Any())
            {
                return NotFound("Categorias não encontradas...");
            }

            categoriasDTO = categorias.MapperParaListaCategoriaDTO();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(15),
                Priority = CacheItemPriority.High
            };

            _cache.Set(CacheCategoriasKey, categoriasDTO, cacheEntryOptions);

            return Ok(categoriasDTO);
        }


        /// <summary>
        /// Obtém uma categoria pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objeto Categoria</returns>
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoriaDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            // teste de excessões
            // throw new Exception("Exceção ao retornar o produto pelo ID");

            // string [] teste = null;
            // if (teste.Length >0 ){}

            var CacheCategoriaKey = $"CacheCategoria_{id}";

            if (_cache.TryGetValue(CacheCategoriaKey, out CategoriaDTO? categoriaDTO))
            {
                return Ok(categoriaDTO);
            }
            _logger.LogInformation($"========= GET categorias/id = {id} ==========");

            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogWarning($"Categoria com id = {id} não foi encontrado");
                return NotFound($"Categoria id = {id} não encontrada...");
            }
            else
            {
                categoriaDTO = categoria.MapperParaCategoriaDTO();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                    SlidingExpiration = TimeSpan.FromSeconds(15),
                    Priority = CacheItemPriority.High
                };

                _cache.Set(CacheCategoriaKey, categoriaDTO, cacheEntryOptions);
                return Ok(categoriaDTO);
            }
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

            var categoriasDto = categorias.MapperParaListaCategoriaDTO();

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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CategoriaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO is null)
                return BadRequest();

            var categoria = categoriaDTO.MapperParaCategoria();

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            await _uof.CommitAsync();

            // Limpa o cache de categorias, pois uma nova categoria foi criada
            _cache.Remove(CacheCategoriasKey);

            var cacheKey = $"CacheCategoria_{categoriaCriada.CategoriaId}";

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(15),
                Priority = CacheItemPriority.High
            };

            var novaCategoriaDTO = categoriaCriada.MapperParaCategoriaDTO();

            _cache.Set(cacheKey, novaCategoriaDTO, cacheEntryOptions);

            return new CreatedAtRouteResult("ObterCategoria",
            new { id = novaCategoriaDTO.CategoriaId }, novaCategoriaDTO);
        }

        [HttpPut("{id:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDTO)
        {
            if (id != categoriaDTO.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos ...");
                return BadRequest("Invalido ...");
            }

            var categoria = categoriaDTO.MapperParaCategoria();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
            await _uof.CommitAsync();

            var novaCategoriaDTO = categoriaAtualizada.MapperParaCategoriaDTO();

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

            var categDTO = categoriaExcluida.MapperParaCategoriaDTO();
            return Ok(categDTO);
        }
    }
}