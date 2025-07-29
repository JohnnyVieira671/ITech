using ITech.Areas.Tecnicos.Services;
using ITech.Context;
using ITech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ITech.Areas.Tecnicos.Controllers
{
    [Area("Tecnicos")]
    public class TecnicosRelatorioVendasController : Controller
    {
        private readonly TecnicosRelatorioVendasService _relatorioVendasService;
        private readonly AppDbContext _context;

        public TecnicosRelatorioVendasController(TecnicosRelatorioVendasService relatorioVendasService, AppDbContext context)
        {
            _context = context;
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


            var email = User.Identity?.Name;

            Tecnico tecnico = await _context.Tecnicos
                       .FirstOrDefaultAsync(p => p.Email == email);

            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            ViewData["Tecnico"] = tecnico.TecnicoNome.ToString();

            var result = await _relatorioVendasService.FindByDateAsync(minDate, maxDate, email);
            return View(result);
        }

        public async Task<IActionResult> RelatorioPrint(DateTime? minDate, DateTime? maxDate, string tecnico)
        {
            var email = User.Identity?.Name;
            
            ViewData["minDate"] = minDate?.ToString("dd/MM/yyyy") ?? "-";
            ViewData["maxDate"] = maxDate?.ToString("dd/MM/yyyy") ?? "-";
            ViewData["Tecnico"] = tecnico;

            var result = await _relatorioVendasService.FindByDateAsync(minDate, maxDate, email);
            return View(result);
        }
    }
}
