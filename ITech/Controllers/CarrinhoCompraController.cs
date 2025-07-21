using ITech.Models;
using ITech.Repositories;
using ITech.Repositories.Interfaces;
using ITech.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ITech.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly IServicoRepository _servicoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraController(CarrinhoCompra carrinhoCompra,
            IServicoRepository servicoRepository)
        {
            _carrinhoCompra = carrinhoCompra;
            _servicoRepository = servicoRepository;
        }

        public IActionResult Index()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItens = itens;

            if(itens.Count <= 0)
            {
                return RedirectToAction("Index", "Home", new { area = (string?)null });
            }

            var carrinhoCompraVM = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = _carrinhoCompra.GetCarrinhoCompraTotal()
            };
            return View(carrinhoCompraVM);
        }

        public IActionResult AdicionarItemNoCarrinhoCompra(int servicoId)
        {
            var servocpSelecionado = _servicoRepository.Servicos.
                FirstOrDefault(l => l.ServicoId == servicoId);
            if(servocpSelecionado != null )
            {
                _carrinhoCompra.AdicionarAoCarrinho(servocpSelecionado);
            }
            return RedirectToAction("Index", "CarrinhoCompra", new { area = (string?)null });
        }

        public IActionResult RemoverItemDoCarrinhoCompras(int id) 
        {
            var servicoSelecionado = _servicoRepository.Servicos.
               FirstOrDefault(l => l.ServicoId == id);

            if (servicoSelecionado != null)
            {
                _carrinhoCompra.RemoverDoCarrinho(servicoSelecionado);
            }
            return RedirectToAction("Index", "CarrinhoCompra", new { area = (string?)null });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AtualizarQuantidade([FromBody] AtualizarQuantidadeDTO dados)
        {
            var servicoSelecionado = _servicoRepository.Servicos
                .FirstOrDefault(s => s.ServicoId == dados.Id);

            if (servicoSelecionado != null)
            {
                _carrinhoCompra.AtualizarQuantidade(servicoSelecionado, dados.Quantidade);
            }

            return Ok();
        }

        public class AtualizarQuantidadeDTO
        {
            public int Id { get; set; }
            public int Quantidade { get; set; }
        }
    }
}
