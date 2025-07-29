using ITech.Areas.Admin.Services;
using ITech.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ITech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminRelatorioVendasController : Controller
    {
        private readonly AdminRelatorioVendasService _relatorioVendasService;
        private readonly AppDbContext _context;

        public AdminRelatorioVendasController(AdminRelatorioVendasService relatorioVendasService, AppDbContext context)
        {
            _relatorioVendasService = relatorioVendasService;
            _context = context;
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

            var result = await _relatorioVendasService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }


        public IActionResult RelatorioPrint(DateTime? minDate, DateTime? maxDate)
        {
            var pedidos = _context.Pedidos
                .Where(p => (!minDate.HasValue || p.PedidoEnviado >= minDate)
                         && (!maxDate.HasValue || p.PedidoEnviado <= maxDate))
                .ToList();

            
            ViewData["minDate"] = minDate?.ToString("dd/MM/yyyy") ?? "-";
            ViewData["maxDate"] = maxDate?.ToString("dd/MM/yyyy") ?? "-";

            return View("RelatorioPrint", pedidos);
        }


    }
}
