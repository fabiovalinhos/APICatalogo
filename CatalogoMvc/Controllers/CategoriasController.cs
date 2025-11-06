using CatalogoMvc.Models;
using CatalogoMvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoMvc.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public async Task<ActionResult<CategoriaViewModel>> Index()
        {
            var result = await _categoriaService.GetCategorias();

            if (result is null)
                return View("Error");

            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}