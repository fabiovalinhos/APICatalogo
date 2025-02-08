using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories
{
    public interface IUnitOfWork
    {
        // IRepository<Produto> ProdutoRepository { get; }
        // IRepository<Categoria> CategoryRepository { get; }

        IProdutoRepository ProdutoRepository { get; }

        ICategoriaRepository CategoriaRepository { get;}

        void Commit();
    }
}