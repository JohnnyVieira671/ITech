using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITech.Context;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;


namespace ITech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminTecnicosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminTecnicosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "TecnicoNome")
        {
            var resultado = _context.Tecnicos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = resultado.Where(p => p.TecnicoNome.Contains(filter) ||
                                                 p.Endereco.Contains(filter) ||
                                                 p.DocIdentificacao.Contains(filter) ||
                                                 p.Telefone.Contains(filter) ||
                                                 p.Email.Contains(filter));
            }

            var model = await PagingList.CreateAsync(resultado, 3, pageindex, sort, "TecnicoNome");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }

        // GET: Admin/Tecnicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tecnico = await _context.Tecnicos
                .FirstOrDefaultAsync(t => t.TecnicoId == id);

            if (tecnico == null)
                return NotFound();

            return View(tecnico);
        }
    }
}
