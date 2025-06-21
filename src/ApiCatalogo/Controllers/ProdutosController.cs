using ApiCatalogo.DTOs;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")] // Todos os métodos deste controller produzem JSON
// [ApiExplorerSettings(IgnoreApi = true)]
[ApiConventionType(typeof(DefaultApiConventions))]//Conversao padrões para todos os métodos
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public ProdutosController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet("produtos/{id}")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int id)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutoPorCategoriaAsync(id);

        if (produtos is null)
            return NotFound();

        return Ok(produtos.MapperParaListaProdutoDTO());
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery]
    ProdutosParameters produtosParameters)
    {
        //produtos seria a própria lista com propriedades
        var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);

        return ObterProdutos(produtos);
    }

    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco(
        [FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutoFiltroPrecoAsync(produtosFiltroPreco);
        return ObterProdutos(produtos);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(PagedList<Models.Produto> produtos)
    {
        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPreview
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDto = produtos.MapperParaListaProdutoDTO();

        return Ok(produtosDto);
    }


    /// <summary>
    /// Exibe uma relação de produtos
    /// </summary>
    /// <returns> Retorna uma lista de objeto Produto</returns>
    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
    {
        try
        {
            // throw new Exception(); serve para testar o tratamento de erro

            var produtos = await _uof.ProdutoRepository.GetAllAsync(null);

            if (produtos is null)
            {
                return NotFound("Produtos não encontrados");
            }

            return Ok(produtos.MapperParaListaProdutoDTO());
        }
        catch
        {
            return BadRequest();
        }

    }

    /// <summary>
    /// Obtém um produto específico pelo id
    /// </summary>
    /// <param name="id">Código do produto</param>
    /// <returns>Um objeto produto</returns>
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int? id)
    {
        if (id is null || id <= 0)
            return BadRequest("ID inválido");


        var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado");
        }

        return Ok(produto.MapperParaProdutoDTO());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProdutoDTO>> Post([FromBody] ProdutoDTO produtoDTO)
    {
        if (produtoDTO is null)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novoProduto = _uof.ProdutoRepository.Create(produtoDTO.MapperParaEntidadeProduto());
        await _uof.CommitAsync();

        var novoProdutoDTO = novoProduto.MapperParaProdutoDTO();

        return new CreatedAtRouteResult(
            "ObterProduto", new { id = novoProdutoDTO.ProdutoId }, novoProdutoDTO);
    }


    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id,
        JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if (patchProdutoDTO is null || id <= 0)
            return BadRequest();

        var produto = await _uof.ProdutoRepository.GetAsync(c => c.ProdutoId == id);

        if (produto is null)
            return NotFound();

        //  TODO: acho que está certo esta volta toda
        // preciso pensar melhor
        var produtoUpdateRequest
            = produto.MapperDeProdutoParaProdutoDTOUpdateRequest();

        patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

        if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        //// Tenho que mandar produto, senão nao tenho tracking
        produto.Estoque = produtoUpdateRequest.Estoque;
        produto.DataCadastro = produtoUpdateRequest.DataCadastro;
        ////

        var produtoFinal = _uof.ProdutoRepository.Update(produto);
        await _uof.CommitAsync();

        return Ok(produtoFinal.MapperDeProdutoParaProdutoDTOUpdateResponse());
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDTO)
    {
        if (id != produtoDTO.ProdutoId)
            return BadRequest("Id com valores diferentes ...");


        var produtoAtualizado = _uof.ProdutoRepository.Update(produtoDTO.MapperParaEntidadeProduto());
        await _uof.CommitAsync();

        return Ok(produtoAtualizado.MapperParaProdutoDTO());
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto is null)
            return NotFound("Produto não encontrado ...");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        await _uof.CommitAsync();

        return Ok(produtoDeletado.MapperParaProdutoDTO());
    }
}