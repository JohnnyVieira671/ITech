using ITech.Context;
using ITech.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITech.Controllers
{
    public class ManipulaTecnicosController : Controller
    {
        private readonly AppDbContext _context;

        public ManipulaTecnicosController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            return View(new Tecnico());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tecnico tecnico)
        {
            if (ModelState.IsValid)
            {
                // Verifica duplicidade de CPF e Email
                bool cpfExists = await _context.Tecnicos
                    .AnyAsync(t => t.DocIdentificacao == tecnico.DocIdentificacao);

                bool emailExists = await _context.Tecnicos
                    .AnyAsync(t => t.Email.ToLower() == tecnico.Email.ToLower());

                if (cpfExists)
                    ModelState.AddModelError("DocIdentificacao", "Já existe um técnico cadastrado com este CPF/CNPJ.");

                if (emailExists)
                    ModelState.AddModelError("Email", "Já existe um técnico cadastrado com este Email.");

                if (cpfExists || emailExists)
                    return View(tecnico);

                // Salva o técnico no banco
                _context.Tecnicos.Add(tecnico);
                await _context.SaveChangesAsync();

                // Atualiza as roles do usuário Identity
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                var user = await userManager.FindByEmailAsync(tecnico.Email);

                if (user != null)
                {
                    if (await userManager.IsInRoleAsync(user, "Member"))
                    {
                        await userManager.RemoveFromRoleAsync(user, "Member");
                    }

                    if (!await userManager.IsInRoleAsync(user, "Tecnicos"))
                    {
                        await userManager.AddToRoleAsync(user, "Tecnicos");
                    }
                }

                // Realiza logout da sessão atual
                await HttpContext.SignOutAsync("Identity.Application");

                // Cria nova identidade com claims de técnico
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, tecnico.Email),
            new Claim("TecnicoId", tecnico.TecnicoId.ToString()),
            new Claim(ClaimTypes.Role, "Tecnicos")
        };

                var identity = new ClaimsIdentity(claims, "Identity.Application");
                var principal = new ClaimsPrincipal(identity);

                // Loga com a nova identidade
                await HttpContext.SignInAsync("Identity.Application", principal);

                // Redireciona para a área de técnicos
                return RedirectToAction("Index", "ManipulaTecnicos", new { area = "Tecnicos" });
            }

            return View(tecnico);
        }




    }
}
