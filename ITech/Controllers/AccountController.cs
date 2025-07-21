using ITech.Context;
using ITech.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace ITech.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AccountController( UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
                                  RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var regexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!regexEmail.IsMatch(loginVM.UserName) && loginVM.UserName.ToLower() != "admin@localhost")
            {
                ModelState.AddModelError("", "O login deve ser feito com um e-mail válido.");
                return View("~/Views/Account/Login.cshtml", loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.UserName);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    // Verifica se email é de técnico
                    bool isTecnico = await _context.Tecnicos.AnyAsync(t => t.Email.ToLower() == loginVM.UserName.ToLower());

                    // Verifica se email é de Adm
                    bool isAdmin = loginVM.UserName.ToLower() == "admin@localhost";

                    // Remove todas roles atuais
                    var userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, userRoles);

                    // Adiciona role conforme tipo
                    if (isTecnico)
                    {
                        if (!await _roleManager.RoleExistsAsync("Tecnicos"))
                            await _roleManager.CreateAsync(new IdentityRole("Tecnicos"));

                        await _userManager.AddToRoleAsync(user, "Tecnicos");
                    }
                    else if (isAdmin)
                    {
                        if (!await _roleManager.RoleExistsAsync("Admin"))
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));

                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        if (!await _roleManager.RoleExistsAsync("Member"))
                            await _roleManager.CreateAsync(new IdentityRole("Member"));

                        await _userManager.AddToRoleAsync(user, "Member");
                    }

                    //if (!string.IsNullOrEmpty(loginVM.ReturnUrl) && Url.IsLocalUrl(loginVM.ReturnUrl))
                    //    return Redirect(loginVM.ReturnUrl);

                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }

            ModelState.AddModelError("", "Falha ao realizar o login!!");
            return View("~/Views/Account/Login.cshtml", loginVM);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel registroVM)
        {
            if (ModelState.IsValid)
            {
                var regexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                // Mesma regra de exceção também pode ser aplicada aqui se quiser permitir registrar admin@localhost
                if (!regexEmail.IsMatch(registroVM.UserName))
                {
                    ModelState.AddModelError("", "O registro deve ser feito com um e-mail válido.");
                    return View(registroVM);
                }

                var user = new IdentityUser
                {
                    UserName = registroVM.UserName,
                    Email = registroVM.UserName
                };

                var result = await _userManager.CreateAsync(user, registroVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member");
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(registroVM);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.User = null;
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
