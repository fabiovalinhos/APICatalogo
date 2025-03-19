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

        public async Task<IEnumerable<Produto>> GetProdutoPorCategoriaAsync(int id)
        {

            var produtosCategoria = await GetAllAsync(c => c.CategoriaId == id);

            return produtosCategoria;
        }

        public async Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters)
        {
            var produtos = await GetAllAsync(null);

            var produtosOrdenados = produtos
                .OrderBy(p => p.ProdutoId)
                .AsQueryable();

            var resultado =
                PagedList<Produto>.ToPagedList(
                    produtosOrdenados, produtosParameters.PageNumber, produtosParameters.PageSize);

            return resultado;
        }

        public async Task<PagedList<Produto>> GetProdutoFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco)
        {
            IEnumerable<Produto> produtos = new List<Produto>();

            if (produtosFiltroPreco.Preco.HasValue &&
                !string.IsNullOrEmpty(produtosFiltroPreco.PrecoCriterio))
            {
                if (produtosFiltroPreco.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = await GetAllAsync(p => p.Preco > produtosFiltroPreco.Preco.Value);
                }
                else if (produtosFiltroPreco.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = await GetAllAsync(p => p.Preco < produtosFiltroPreco.Preco.Value);
                }
                else if (produtosFiltroPreco.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    produtos = await GetAllAsync(p => p.Preco == produtosFiltroPreco.Preco.Value);
                }
            }
            else
            {
                produtos = await GetAllAsync(null);
            }

            produtos = produtos.OrderBy(p => p.Preco);

            var produtosFiltradosEPaginados =
                PagedList<Produto>.ToPagedList(
                    produtos.AsQueryable(), produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);

            return produtosFiltradosEPaginados;
        }
    }
}