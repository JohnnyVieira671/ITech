using ITech.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITech.Models
{
    public class PedidoDetalhe
    {
        public int PedidoDetalheId { get; set; }
        public int PedidoId { get; set; }
        public int ServicoId { get; set; }
        public int Quantidade { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco {  get; set; }

        public virtual Servico Servico { get; set; }
        public virtual Pedido Pedido { get; set; }
    }
}
