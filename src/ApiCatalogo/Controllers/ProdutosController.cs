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
// [ApiExplorerSettings(IgnoreApi = true)]
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

        return Ok(produtos.ParaProdutoListaMapper());
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

        var produtosDto = produtos.ParaProdutoListaMapper();

        return Ok(produtosDto);
    }

    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
    {
        var produtos = await _uof.ProdutoRepository.GetAllAsync(null);

        if (produtos is null)
        {
            return NotFound("Produtos não encontrados");
        }

        return Ok(produtos.ParaProdutoListaMapper());
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado");
        }

        return Ok(produto.ParaProdutoDTOMapper());
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDTO>> Post([FromBody] ProdutoDTO produtoDTO)
    {
        if (produtoDTO is null)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novoProduto = _uof.ProdutoRepository.Create(produtoDTO.ParaEntidadeProdutoMapper());
        await _uof.CommitAsync();

        var novoProdutoDTO = novoProduto.ParaProdutoDTOMapper();

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
            = produto.DeProdutoParaProdutoDTOUpdateRequestMapper();

        patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

        if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        /// Tenho que mandar produto, senão nao tenho tracking
        produto.Estoque = produtoUpdateRequest.Estoque;
        produto.DataCadastro = produtoUpdateRequest.DataCadastro;
        ///

        var produtoFinal = _uof.ProdutoRepository.Update(produto);
        await _uof.CommitAsync();

        return Ok(produtoFinal.DeProdutoParaProdutoDTOUpdateResponseMapper());
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDTO)
    {
        if (id != produtoDTO.ProdutoId)
            return BadRequest("Id com valores diferentes ...");


        var produtoAtualizado = _uof.ProdutoRepository.Update(produtoDTO.ParaEntidadeProdutoMapper());
        await _uof.CommitAsync();

        return Ok(produtoAtualizado.ParaProdutoDTOMapper());
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

        if (produto is null)
            return NotFound("Produto não encontrado ...");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        await _uof.CommitAsync();

        return Ok(produtoDeletado.ParaProdutoDTOMapper());
    }
}