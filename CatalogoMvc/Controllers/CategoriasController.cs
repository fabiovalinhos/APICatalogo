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


        [HttpGet]
        public IActionResult CriarNovaCategoria()
        {
            return View();
        }
        

        [HttpPost]
        public async Task<ActionResult<CategoriaViewModel>> CriarNovaCategoria(CategoriaViewModel categoria)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoriaService.CriarCategoria(categoria);

                if (result is not null)
                    return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "Erro ao criar a categoria. Tente novamente.";
            return View(categoria);
        }
    }
}