using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITech.Context;
using ITech.Models;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITech.Areas.Tecnicos.Controllers
{
    [Area("Tecnicos")]
    public class TecnicosServico : Controller
    {
    
        private readonly AppDbContext _context;

        public TecnicosServico(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "DescricaoCurta")
        {
                var email = User.Identity?.Name;
                var resultado = _context.Servicos
                    .Include(s => s.Categoria)
                    .Include(s => s.Tecnicos)
                    .AsNoTracking()
                    .Where(s => s.Tecnicos.Email == email); 

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    resultado = resultado.Where(p =>
                        p.DescricaoCurta.Contains(filter) ||
                        p.DescricaoDetalhada.Contains(filter) ||
                        p.Valor.ToString().Contains(filter) ||
                        p.Categoria.CategoriaNome.Contains(filter) ||
                        p.Tecnicos.TecnicoNome.Contains(filter));
                }

                var model = await PagingList.CreateAsync(resultado, 5, pageindex, sort, "DescricaoCurta");
                model.RouteValue = new RouteValueDictionary 
                {
                    { "filter", filter },
                    { "area", "Tecnicos" }
                };

            return View(model);
        }




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

        
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome");
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "TecnicoId", "TecnicoNome");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicoId,DescricaoCurta,DescricaoDetalhada,Valor,EmDisposicao,CategoriaId")] Servico servico)
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.Email == email);
            if (tecnico == null)
                return Unauthorized();

            // Atribui o Técnico logado ao serviço
            servico.TecnicoId = tecnico.TecnicoId;

            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "TecnicosServico", new { area = "Tecnicos" });
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", servico.CategoriaId);
            return View(servico);
        }


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
                return RedirectToAction(nameof(Index), "TecnicosServico", new { area = "Tecnicos" });
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "CategoriaNome", servico.CategoriaId);
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "TecnicoId", "TecnicoNome", servico.TecnicoId);
            return View(servico);
        }

        
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
                var itensCarrinho = _context.CarrinhoCompraItens.Where(i => i.Servico.ServicoId == id);
                _context.CarrinhoCompraItens.RemoveRange(itensCarrinho);
                _context.Servicos.Remove(servico);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), "TecnicosServico", new { area = "Tecnicos" });
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.ServicoId == id);
        }
    }
}
