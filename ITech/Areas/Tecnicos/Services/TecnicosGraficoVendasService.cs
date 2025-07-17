using ITech.Context;
using ITech.Models;
using Microsoft.EntityFrameworkCore;

namespace ITech.Areas.Tecnicos.Services
{
    public class TecnicosGraficoVendasService
    {
        private readonly AppDbContext _context;

        public TecnicosGraficoVendasService(AppDbContext context)
        {
            _context = context;
        }

        public List<ServicoGrafico> GetVendasServicos(int dias, string email)
        {
            var dataLimite = DateTime.Today.AddDays(-dias);

            var resultado = _context.PedidoDetalhes
                .Include(p => p.Pedido)
                .Include(p => p.Servico)
                    .ThenInclude(s => s.Tecnicos)
                .Where(p => p.Pedido.PedidoEnviado >= dataLimite && p.Servico.Tecnicos.Email == email)
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
