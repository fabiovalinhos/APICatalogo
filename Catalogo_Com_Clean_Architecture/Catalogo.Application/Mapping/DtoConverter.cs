using Catalogo.Application.DTOs;
using Catalogo.Domain.Entities;

namespace Catalogo.Application.Mapping;

public static class DtoConverter
{
    public static CategoriaDTO ToCategoriaDto(Categoria categoria)
    {
        if (categoria == null) return null;
        return new CategoriaDTO
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            ImagemUrl = categoria.ImagemUrl
        };
    }

    public static Categoria ToCategoria(CategoriaDTO categoriaDto)
    {
        if (categoriaDto == null) return null;
        return new Categoria(
            categoriaDto.Id,
            categoriaDto.Nome,
            categoriaDto.ImagemUrl
        );

    }

    public static ProdutoDTO ToProdutoDto(Produto produto)
    {
        if (produto == null) return null;
        return new ProdutoDTO
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            ImagemUrl = produto.ImagemUrl,
            Estoque = produto.Estoque,
            DataCadastro = produto.DataCadastro,
            CategoriaId = produto.CategoriaId
        };
    }

    public static Produto ToProduto(ProdutoDTO produtoDto)
    {
        if (produtoDto == null) return null;
        var produto = new Produto(
            produtoDto.Id,
            produtoDto.Nome,
            produtoDto.Descricao,
            produtoDto.Preco,
            produtoDto.ImagemUrl,
            produtoDto.Estoque,
            produtoDto.DataCadastro
        );
        
        return produto;
    }
}
