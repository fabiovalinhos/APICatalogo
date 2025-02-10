using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers;

[ApiController]
[Route("[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public ProdutosController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    public ProdutosController()
    {
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<Produto>> GetProdutosPorCategoria(int id)
    {
        var produtos = _uof.ProdutoRepository.GetProdutoPorCategoria(id);

        if (produtos is null)
            return NotFound();

        return Ok(produtos);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _uof.ProdutoRepository.GetAll();

        if (produtos is null)
        {
            return NotFound("Produtos não encontrados");
        }

        return Ok(produtos);
    }

    [HttpGet("id:int:min(1)", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado");
        }

        return Ok(produto);
    }

    [HttpPost]
    public ActionResult Post([FromBody] Produto produto)
    {
        if (produto is null)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novoProduto = _uof.ProdutoRepository.Create(produto);
        _uof.Commit();

        return new CreatedAtRouteResult(
            "ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
            return BadRequest();


        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
        _uof.Commit();

        return Ok(produtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

        if (produto is null)
            return NotFound("Produto não encontrado ...");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        _uof.Commit();

        return Ok(produtoDeletado);
    }
}