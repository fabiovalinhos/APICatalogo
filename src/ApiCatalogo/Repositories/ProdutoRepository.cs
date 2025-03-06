using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Produto> GetProdutoPorCategoria(int id)
        {
            return GetAll().Where(c => c.CategoriaId == id);
        }

        public PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters)
        {
            var produtos = GetAll()
                .OrderBy(p => p.ProdutoId)
                .AsQueryable();

            var produtosOrdenados =
                PagedList<Produto>.ToPagedList(
                    produtos, produtosParameters.PageNumber, produtosParameters.PageSize);

            return produtosOrdenados;
        }

        public PagedList<Produto> GetProdutoFiltroPreco(ProdutosFiltroPreco produtosFiltroPreco)
        {
            var produtos =
               GetAll()
               .AsQueryable();

            if (produtosFiltroPreco.Preco.HasValue &&
                !string.IsNullOrEmpty(produtosFiltroPreco.PrecoCriterio))
            {
                if (produtosFiltroPreco.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco > produtosFiltroPreco.Preco.Value)
                        .OrderBy(p => p.Preco);
                }
                else if (produtosFiltroPreco.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco < produtosFiltroPreco.Preco.Value)
                        .OrderBy(p => p.Preco);
                }
                else if (produtosFiltroPreco.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = produtos.Where(p => p.Preco == produtosFiltroPreco.Preco.Value)
                        .OrderBy(p => p.Preco);
                }
            }

            var produtosFiltradosEPaginados =
                PagedList<Produto>.ToPagedList(
                    produtos, produtosFiltroPreco.PageNumber,produtosFiltroPreco.PageSize);

            return produtosFiltradosEPaginados;
        }
    }
}