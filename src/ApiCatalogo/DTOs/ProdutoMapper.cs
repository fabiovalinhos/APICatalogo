using ApiCatalogo.Models;

namespace ApiCatalogo.DTOs
{
    public static class ProdutoMapper
    {
        public static ProdutoDTO? MapperParaProdutoDTO(this Produto entidade)
        {
            if (entidade is null) return null;

            return new ProdutoDTO
            {
                ProdutoId = entidade.ProdutoId,

                Nome = entidade.Nome,
                Descricao = entidade.Descricao,
                Preco = entidade.Preco,
                ImageUrl = entidade.ImageUrl,
                CategoriaId = entidade.CategoriaId
            };
        }

        public static Produto? MapperParaEntidadeProduto(this ProdutoDTO entidade)
        {
            if (entidade is null) return null;

            return new Produto
            {
                ProdutoId = entidade.ProdutoId,

                Nome = entidade.Nome,
                Descricao = entidade.Descricao,
                Preco = entidade.Preco,
                ImageUrl = entidade.ImageUrl,
                CategoriaId = entidade.CategoriaId
            };
        }

        public static IEnumerable<ProdutoDTO> MapperParaListaProdutoDTO(this IEnumerable<Produto> entidade)
        {
            if (entidade is null || !entidade.Any())
            {
                return new List<ProdutoDTO>();
            }

            return entidade.Select(prod => new ProdutoDTO
            {
                ProdutoId = prod.ProdutoId,

                Nome = prod.Nome,
                Descricao = prod.Descricao,
                Preco = prod.Preco,
                ImageUrl = prod.ImageUrl,
                CategoriaId = prod.CategoriaId
            }).ToList();
        }


        public static ProdutoDTOUpdateRequest? MapperDeProdutoParaProdutoDTOUpdateRequest(this Produto entidade)
        {
            if (entidade is null) return null;

            return new ProdutoDTOUpdateRequest
            {
                Estoque = entidade.Estoque,
                DataCadastro = entidade.DataCadastro
            };
        }

        public static Produto? MapperDeProdutoDTOUpdateRequestParaProduto(this ProdutoDTOUpdateRequest entidade)
        {
            if (entidade is null) return null;

            return new Produto
            {
                Estoque = entidade.Estoque,
                DataCadastro = entidade.DataCadastro
            };
        }

        public static ProdutoDTOUpdateResponse? MapperDeProdutoParaProdutoDTOUpdateResponse(this Produto entidade)
        {
            if (entidade is null) return null;

            return new ProdutoDTOUpdateResponse
            {
                ProdutoId = entidade.ProdutoId,
                Nome = entidade.Nome,
                Descricao = entidade.Descricao,
                Preco = entidade.Preco,
                ImageUrl = entidade.ImageUrl,
                Estoque = entidade.Estoque,
                DataCadastro = entidade.DataCadastro,
                CategoriaId = entidade.CategoriaId,
                Categoria = entidade.Categoria
            };
        }

        public static Produto? MapperDeProdutoDTOUpdateResponseParaProduto(this ProdutoDTOUpdateResponse entidade)
        {
            if (entidade is null) return null;

            return new Produto
            {
                ProdutoId = entidade.ProdutoId,
                Nome = entidade.Nome,
                Descricao = entidade.Descricao,
                Preco = entidade.Preco,
                ImageUrl = entidade.ImageUrl,
                Estoque = entidade.Estoque,
                DataCadastro = entidade.DataCadastro,
                CategoriaId = entidade.CategoriaId,
                Categoria = entidade.Categoria
            };
        }
    }
}