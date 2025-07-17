using ITech.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ITech.Repositories; // ajuste o namespace
using System.Linq;

namespace ITech.ViewComponents
{
    public class CategoriaMenuViewComponent : ViewComponent
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaMenuViewComponent(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public IViewComponentResult Invoke()
        {
            var categorias = _categoriaRepository.Categorias.OrderBy(c => c.CategoriaNome);
            return View(categorias);
        }
    }
}
