using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public interface IProdutoRepository: IRepository<Produto>
    {
        PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters);

        IEnumerable<Produto> GetProdutoPorCategoria(int id);
    }
}