using System.Text.Json;
using System.IO;
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
            var client = _clientFactory.CreateClient("CategoriasApi");

            using (var response = await client.PutAsJsonAsync(apiEndpoint + id, categoria))
            {
                if(response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<CategoriaViewModel> CriarCategoria(CategoriaViewModel categoria)
        {
         var client = _clientFactory.CreateClient("CategoriasApi");

            StringContent categoriaJson = new StringContent(
                JsonSerializer.Serialize(categoria),
                System.Text.Encoding.UTF8,
                "application/json");

            using (var response = await client.PostAsync(apiEndpoint, categoriaJson))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    categoriaVM = JsonSerializer.Deserialize<CategoriaViewModel>(apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }

            return categoriaVM;   
        }

        public async Task<bool> DeletaCategoria(int id)
        {
            var client = _clientFactory.CreateClient("CategoriasApi");

            using (var response = await client.DeleteAsync(apiEndpoint + id))
            {
                if(response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<CategoriaViewModel> GetCategoriaPorId(int id)
        {
            var client = _clientFactory.CreateClient("CategoriasApi");

            using (var response = await client.GetAsync(apiEndpoint + id))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    categoriaVM = JsonSerializer.Deserialize<CategoriaViewModel>(apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }

            return categoriaVM;
        }

        public async Task<IEnumerable<CategoriaViewModel>> GetCategorias()
        {
            var client = _clientFactory.CreateClient("CategoriasApi");

            using (var response = await client.GetAsync(apiEndpoint))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    categoriasVM = JsonSerializer.Deserialize<IEnumerable<CategoriaViewModel>>(apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }

            return categoriasVM;
        }
    }
}
