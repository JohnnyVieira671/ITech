using System.Diagnostics;
using System.Linq;
using ITech.Models;
using ITech.Repositories.Interfaces;
using ITech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ITech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServicoRepository _servicoRepository;

        public HomeController(ILogger<HomeController> logger, IServicoRepository servicoRepository)
        {
            _logger = logger;
            _servicoRepository = servicoRepository;
        }


        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                ServicosPreferidos = _servicoRepository
                    .GetServicosMaisVendidos(3)
                    .ToList()
            };

            return View(homeViewModel);
        }


        //public IActionResult Index()
        //{
        //    var homeViewModel = new HomeViewModel
        //    {
        //        ServicosPreferidos = _servicoRepository
        //            .Servicos
        //            .Where(s => s.IsPreferido)
        //            .ToList()
        //    };

        //    return View(homeViewModel);
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
