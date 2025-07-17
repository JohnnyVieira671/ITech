using ITech.Context;
using ITech.Models;
using Microsoft.EntityFrameworkCore;

namespace ITech.Areas.Admin.Services
{
    public class AdminGraficoVendasService
    {
        private readonly AppDbContext _context;

        public AdminGraficoVendasService(AppDbContext context)
        {
            _context = context;
        }

        public List<ServicoGrafico> GetVendasServicos(int dias)
        {
            var dataLimite = DateTime.Today.AddDays(-dias);

            var resultado = _context.PedidoDetalhes
                .Include(p => p.Pedido)
                .Include(p => p.Servico)
                .Where(p => p.Pedido.PedidoEnviado >= dataLimite)
                .GroupBy(p => p.Servico.DescricaoCurta)
                .Select(g => new ServicoGrafico
                {
                    DescricaoCurta = g.Key,
                    ServicoQuantidade = g.Sum(s => s.Quantidade),
                    ServicoValorTotal = (int)g.Sum(s => s.Quantidade * s.Preco)
                })
                .Where(g => g.ServicoQuantidade > 0)
                .OrderByDescending(g => g.ServicoQuantidade)
                .ToList();

            return resultado;
        }

    }
}
