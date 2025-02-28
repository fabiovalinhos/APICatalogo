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
            var produtos =  GetAll()
                .OrderBy(p => p.ProdutoId)
                .AsQueryable();

            var produtosOrdenados =
                PagedList<Produto>.ToPagedList(
                    produtos,produtosParameters.PageNumber,produtosParameters.PageSize);

            return produtosOrdenados;
        }
    }
}