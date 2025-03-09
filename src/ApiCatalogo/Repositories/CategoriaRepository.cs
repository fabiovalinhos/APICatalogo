using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {

        }

        public PagedList<Categoria> GetCategorias(CategoriasParameters categoriasParameters)
        {
            var categorias = GetAll()
                .OrderBy(p => p.CategoriaId)
                .AsQueryable();

            var categoriasOrdenados =
                PagedList<Categoria>.ToPagedList(
                    categorias, categoriasParameters.PageNumber, categoriasParameters.PageSize);

            return categoriasOrdenados;
        }

        public PagedList<Categoria> GetCategoriasFiltroNome(CategoriasFiltroNome categoriasFiltroNome)
        {
            var categorias = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
            {
                categorias = categorias.Where(c => c.Nome.Contains(categoriasFiltroNome.Nome));
            }

            var categoriasFiltradas =
            PagedList<Categoria>.ToPagedList(
                categorias, categoriasFiltroNome.PageNumber, categoriasFiltroNome.PageSize
            );

            return categoriasFiltradas;
        }
    }
}
