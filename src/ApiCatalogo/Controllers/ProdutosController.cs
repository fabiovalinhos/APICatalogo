using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers;

[ApiController]
[Route("[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _repository;

    public ProdutosController(IProdutoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _repository.GetProdutos().ToList();

        if (produtos is null)
        {
            return NotFound("Produtos não encontrados");
        }

        return Ok(produtos);
    }

    [HttpGet("id:int:min(1)", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _repository.GetProduto(id);

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

        var novoProduto = _repository.Create(produto);

        return new CreatedAtRouteResult(
            "ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
            return BadRequest();


        bool atualizado = _repository.Update(produto);
        if (atualizado)
        {
            return Ok(produto);
        }

        return StatusCode(500, $"Falha em atualizar o produto de id = {id}");
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var deletado = _repository.Delete(id);
        if (deletado)
        {
            return Ok($"Produto de id = {id} foi excluído");
        }

        return StatusCode(500, $"Falha ao deletar o produto de id = {id}");
    }
}