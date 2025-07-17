using ITech.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITech.Areas.Tecnicos.Controllers
{
    [Area("Tecnicos")]
    //[Authorize(Roles = "Tecnicos")]
    public class TecnicosController : Controller
    {
        private readonly AppDbContext _context;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TodosOsTecnicos()
        {
            return View();
        }
    }
}
