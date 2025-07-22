using ITech.Context;
using ITech.Models;
using ITech.Repositories.Interfaces;

namespace ITech.Repositories
{
    public class PedidoRepository: IPedidoRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoRepository(AppDbContext appDbContext,
            CarrinhoCompra carrinhoCompra)
        {
            _carrinhoCompra = carrinhoCompra;
            _appDbContext = appDbContext;
        }

        public void CriarPedido(Pedido pedido)
        {
            pedido.PedidoEnviado = DateTime.Now;
            _appDbContext.Pedidos.Add(pedido);
            _appDbContext.SaveChanges();

            var carrinhoCompraItens = _carrinhoCompra.CarrinhoCompraItens;

            foreach(var carrinhoItem in carrinhoCompraItens)
            {
                var pedidoDetail = new PedidoDetalhe() 
                {
                    Quantidade = carrinhoItem.Quantidade,
                    ServicoId = carrinhoItem.Servico.ServicoId,
                    PedidoId = pedido.PedidoId,
                    Preco = carrinhoItem.Servico.Valor
                };
                _appDbContext.PedidoDetalhes.Add(pedidoDetail);
            }
            _appDbContext.SaveChanges();
        }



    }
}












