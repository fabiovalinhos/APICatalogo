using CatalogoMvc.Models;

namespace CatalogoMvc.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaViewModel>> GetCategorias();
        Task<CategoriaViewModel> GetCategoriaPorId(int id);
        Task<CategoriaViewModel> CriarCategoria(CategoriaViewModel categoria);
        Task<bool> AtualizarCategoria(int id, CategoriaViewModel categoria);
        Task<bool> DeletarCategoria(int id);
    }
}