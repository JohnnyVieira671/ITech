using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITech.Context;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using ITech.Models;

namespace ITech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminTecnicosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AdminTecnicosController(AppDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Admin/Tecnicos/Index
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

            var model = await PagingList.CreateAsync(resultado, 5, pageindex, sort, "TecnicoNome");
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

        // GET: Admin/Tecnicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tecnico = await _context.Tecnicos.FirstOrDefaultAsync(t => t.TecnicoId == id);

            if (tecnico == null)
                return NotFound();

            return View(tecnico);  // Exibe a página de confirmação inicial
        }

        // POST: Admin/Tecnicos/DeleteConfirmed/5
        // Confirmação de exclusão antes de pedir a senha
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tecnico = _context.Tecnicos.Find(id);
            if (tecnico == null)
                return NotFound();

            // Redireciona para a confirmação da senha
            return RedirectToAction("ConfirmDelete", new { id });
        }

        // GET: Admin/Tecnicos/ConfirmDelete/5
        public IActionResult ConfirmDelete(int id)
        {
            var tecnico = _context.Tecnicos.Find(id);
            if (tecnico == null)
                return NotFound();

            return View(tecnico);  // Exibe o formulário para o admin inserir a senha
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "A senha não pode ser vazia.");
                var tecnicoToDelete = _context.Tecnicos.Find(id);
                return View(tecnicoToDelete);
            }

            var admin = await _userManager.GetUserAsync(User);
            var result = await _signInManager.PasswordSignInAsync(admin, password, false, false);

            if (result.Succeeded)
            {
                var tecnico = await _context.Tecnicos.FindAsync(id);
                if (tecnico != null)
                {
                    var user = await _userManager.FindByEmailAsync(tecnico.Email);
                    if (user != null)
                    {
                        if (await _userManager.IsInRoleAsync(user, "Tecnicos"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Tecnicos");
                        }

                        await _userManager.DeleteAsync(user);
                    }

                    var carrinhoItens = _context.CarrinhoCompraItens
                        .Where(ci => ci.Servico.ServicoId == tecnico.TecnicoId);
                    _context.CarrinhoCompraItens.RemoveRange(carrinhoItens);

                    _context.Tecnicos.Remove(tecnico);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "AdminTecnicos", new { area = "Admin" });
            }

            // Passa a mensagem de erro para a view se a senha estiver incorreta
            ViewData["ErrorMessage"] = "Senha incorreta.";
            var tecnicoToDeleteError = _context.Tecnicos.Find(id);
            return View(tecnicoToDeleteError);
        }

    }
}
