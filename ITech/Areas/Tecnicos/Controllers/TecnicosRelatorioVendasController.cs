using ITech.Areas.Tecnicos.Services;
using Microsoft.AspNetCore.Mvc;


namespace ITech.Areas.Tecnicos.Controllers
{
    [Area("Tecnicos")]
    public class TecnicosRelatorioVendasController : Controller
    {
        private readonly TecnicosRelatorioVendasService _relatorioVendasService;

        public TecnicosRelatorioVendasController(TecnicosRelatorioVendasService relatorioVendasService)
        {
            _relatorioVendasService = relatorioVendasService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RelatorioVendasSimples(
            DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }

            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");

            var email = User.Identity?.Name;

            var result = await _relatorioVendasService.FindByDateAsync(minDate, maxDate, email);
            return View(result);
        }
    }
}
