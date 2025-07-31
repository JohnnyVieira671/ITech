using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITech.Context;
using ITech.Models;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ITech.Areas.Tecnicos.Controllers
{
    [Area("Tecnicos")]
   // [Authorize(Roles = "Tecnicos")]
    public class ManipulaTecnicosController : Controller
    {
        private readonly AppDbContext _context;

        public ManipulaTecnicosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Tecnicos
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "TecnicoNome")
        {
            var resultado = _context.Tecnicos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = resultado.Where(t =>
                    t.TecnicoNome.Contains(filter) ||
                    t.Email.Contains(filter) ||
                    t.Telefone.Contains(filter) ||
                    t.DocIdentificacao.Contains(filter) ||
                    t.Endereco.Contains(filter));
            }

            var model = await PagingList.CreateAsync(resultado, 10, pageindex, sort, "TecnicoNome");
            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }

        




        // GET: Admin/Tecnicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tecnico = await _context.Tecnicos.FindAsync(id);

            if (tecnico == null)
                return NotFound();

            return View(tecnico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tecnico tecnico)
        {
            if (id != tecnico.TecnicoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Verifica CPF/CNPJ duplicado em outro registro
                bool cpfExists = await _context.Tecnicos
                    .AnyAsync(t => t.DocIdentificacao == tecnico.DocIdentificacao && t.TecnicoId != tecnico.TecnicoId);

                // Verifica Email duplicado em outro registro
                bool emailExists = await _context.Tecnicos
                    .AnyAsync(t => t.Email.ToLower() == tecnico.Email.ToLower() && t.TecnicoId != tecnico.TecnicoId);

                if (cpfExists)
                    ModelState.AddModelError("DocIdentificacao", "Já existe outro técnico cadastrado com este CPF/CNPJ.");

                if (emailExists)
                    ModelState.AddModelError("Email", "Já existe outro técnico cadastrado com este Email.");

                if (cpfExists || emailExists)
                    return View(tecnico);

                try
                {
                    _context.Update(tecnico);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "ManipulaTecnicos", new { area = "Tecnicos" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TecnicoExists(tecnico.TecnicoId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(tecnico);
        }

        // GET: Admin/Tecnicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.TecnicoId == id);

            if (tecnico == null)
                return NotFound();

            return View(tecnico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico != null)
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                var user = await userManager.FindByEmailAsync(tecnico.Email); 

                if (user != null)
                {
                    if (await userManager.IsInRoleAsync(user, "Tecnicos"))
                    {
                        await userManager.RemoveFromRoleAsync(user, "Tecnicos");
                    }

                    await userManager.DeleteAsync(user);

                    await HttpContext.SignOutAsync("Identity.Application");

                }
                _context.Tecnicos.Remove(tecnico);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home", new { area = (string)null });
        }



        private bool TecnicoExists(int id)
        {
            return _context.Tecnicos.Any(t => t.TecnicoId == id);
        }
    }
}
