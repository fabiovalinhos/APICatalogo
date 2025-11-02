using System.Text.Json;
using CatalogoMvc.Models;

namespace CatalogoMvc.Services
{
    public class CategoriaService : ICategoriaService
    {
        private const string apiEndpoint = "/api/1/categorias/";
        private readonly JsonSerializerOptions _options;
        private readonly IHttpClientFactory _clientFactory;

        private CategoriaViewModel categoriaVM;
        private IEnumerable<CategoriaViewModel> categoriasVM;


        public CategoriaService(IHttpClientFactory clientFactory)
        {
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _clientFactory = clientFactory;
        }



        public async Task<bool> AtualizarCategoria(int id, CategoriaViewModel categoria)
        {
            throw new NotImplementedException();
        }

        public async Task<CategoriaViewModel> CriarCategoria(CategoriaViewModel categoria)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeletarCategoria(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CategoriaViewModel> GetCategoriaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CategoriaViewModel>> GetCategorias()
        {
            throw new NotImplementedException();
        }
    }
}