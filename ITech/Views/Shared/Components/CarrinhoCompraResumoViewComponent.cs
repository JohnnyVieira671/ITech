using ITech.Models;
using ITech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ITech.Models; // ajuste se necessário
using ITech.Repositories; // ajuste se necessário

namespace ITech.ViewComponents
{
    public class CarrinhoCompraResumoViewComponent : ViewComponent
    {
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraResumoViewComponent(CarrinhoCompra carrinhoCompra)
        {
            _carrinhoCompra = carrinhoCompra;
        }

        public IViewComponentResult Invoke()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItens = itens;

            var carrinhoCompraViewModel = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = _carrinhoCompra.GetCarrinhoCompraTotal()
            };

            return View(carrinhoCompraViewModel);
        }
    }
}
