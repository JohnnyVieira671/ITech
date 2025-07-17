using ITech.Models;

namespace ITech.ViewModels
{
    public class PedidoServicoViewModel
    {
        public Pedido Pedido { get; set; }
        public IEnumerable<PedidoDetalhe> PedidoDetalhes { get; set; }
    }
}
