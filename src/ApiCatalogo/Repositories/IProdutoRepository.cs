using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public interface IProdutoRepository: IRepository<Produto>
    {
        PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters);

        PagedList<Produto> GetProdutoFiltroPreco(ProdutosFiltroPreco produtosFiltroPreco);

        IEnumerable<Produto> GetProdutoPorCategoria(int id);
    }
}