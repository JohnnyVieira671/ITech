using ITech.Areas.Tecnicos.Services;
using ITech.ViewModels; // Use o ViewModel correto para serviços, se estiver em outra pasta, ajuste o namespace
using Microsoft.AspNetCore.Mvc;
using System;

namespace ITech.Areas.Tecnicos.Controllers
{
    [Area("Tecnicos")]
    public class TecnicosGraficoController : Controller
    {
        private readonly TecnicosGraficoVendasService _graficoVendas;

        public TecnicosGraficoController(TecnicosGraficoVendasService graficoVendas)
        {
            _graficoVendas = graficoVendas ?? throw new ArgumentNullException(nameof(graficoVendas));
        }

        /// <summary>
        /// Retorna os dados em JSON para o gráfico de vendas de serviços.
        /// </summary>
        /// <param name="dias">Intervalo de dias a consultar (ex: 30, 7, etc).</param>
        [HttpGet]
        public JsonResult VendasServicos(int dias)
        {
            var email = User.Identity?.Name;

            var servicosVendasTotais = _graficoVendas.GetVendasServicos(dias, email);

            if (servicosVendasTotais == null || !servicosVendasTotais.Any())
            {
                return Json(new { success = false, message = "Nenhum dado encontrado para o período informado." });
            }

            return Json(servicosVendasTotais);
        }

        /// <summary>
        /// View principal de gráficos (ex: gráfico anual ou geral).
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// View com gráfico mensal (últimos X dias).
        /// </summary>
        [HttpGet]
        public IActionResult VendasMensal(int dias = 30)
        {
            ViewBag.Dias = dias;
            return View();
        }

        /// <summary>
        /// View com gráfico semanal (últimos 7 dias, por padrão).
        /// </summary>
        [HttpGet]
        public IActionResult VendasSemanal(int dias = 7)
        {
            ViewBag.Dias = dias;
            return View();
        }
    }
}
