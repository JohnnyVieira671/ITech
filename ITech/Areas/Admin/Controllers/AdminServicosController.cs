using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITech.Context;
using ITech.Models;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;

namespace ITech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminServicosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminServicosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminServicos
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "DescricaoCurta")
        {
            var resultado = _context.Servicos
                .Include(s => s.Categoria)
                .Include(s => s.Tecnicos)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = resultado.Where(p => p.DescricaoCurta.Contains(filter) ||
                                                 p.DescricaoDetalhada.Contains(filter) ||
                                                 p.Valor.ToString().Contains(filter) ||
                                                 p.Categoria.CategoriaNome.Contains(filter) ||
                                                 p.Tecnicos.TecnicoNome.Contains(filter));
            }

            var model = await PagingList.CreateAsync(resultado, 3, pageindex, sort, "DescricaoCurta");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }

        // GET: Admin/AdminServicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Servicos == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .Include(s => s.Categoria)
                .Include(s => s.Tecnicos)
                .FirstOrDefaultAsync(m => m.ServicoId == id);

            if (servico == null)
            {
                return NotFound();
            }

            return View(servico);
        }

        // GET: Admin/AdminServicos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome");
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "TecnicoId", "TecnicoNome");
            return View();
        }

        // POST: Admin/AdminServicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicoId,DescricaoCurta,DescricaoDetalhada,Valor,EmDisposicao,CategoriaId,TecnicoId")] Servico servico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", servico.CategoriaId);
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "TecnicoId", "TecnicoNome", servico.TecnicoId);
            return View(servico);
        }

        // GET: Admin/AdminServicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Servicos == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
            {
                return NotFound();
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", servico.CategoriaId);
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "TecnicoId", "TecnicoNome", servico.TecnicoId);
            return View(servico);
        }

        // POST: Admin/AdminServicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicoId,DescricaoCurta,DescricaoDetalhada,Valor,EmDisposicao,CategoriaId,TecnicoId")] Servico servico)
        {
            if (id != servico.ServicoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoExists(servico.ServicoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", servico.CategoriaId);
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "TecnicoId", "TecnicoNome", servico.TecnicoId);
            return View(servico);
        }

        // GET: Admin/AdminServicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Servicos == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .Include(s => s.Categoria)
                .Include(s => s.Tecnicos)
                .FirstOrDefaultAsync(m => m.ServicoId == id);
            if (servico == null)
            {
                return NotFound();
            }

            return View(servico);
        }

        // POST: Admin/AdminServicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Servicos == null)
            {
                return Problem("Entity set 'AppDbContext.Servicos' is null.");
            }

            var servico = await _context.Servicos.FindAsync(id);
            if (servico != null)
            {
                _context.Servicos.Remove(servico);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.ServicoId == id);
        }
    }
}
