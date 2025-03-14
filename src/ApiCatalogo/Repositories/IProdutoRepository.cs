using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters);

        Task<PagedList<Produto>> GetProdutoFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco);

        Task<IEnumerable<Produto>> GetProdutoPorCategoriaAsync(int id);
    }
}