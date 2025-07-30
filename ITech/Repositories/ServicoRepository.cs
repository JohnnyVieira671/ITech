using ITech.Context;
using ITech.Models;
using ITech.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITech.Repositories
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly AppDbContext _context;

        public ServicoRepository(AppDbContext contexto)
        {
            _context = contexto;
        }

        // Agora permite aplicar filtros antes de executar a consulta
        public IQueryable<Servico> Servicos =>
            _context.Servicos.Include(c => c.Categoria);

        //public IEnumerable<Servico> ServicosPreferidos =>
        //    _context.Servicos
        //        .Where(p => p.IsPreferido)
        //        .Include(c => c.Categoria)
        //        .ToList();

        public Servico GetlancheById(int lancheId)
        {
            return _context.Servicos.FirstOrDefault(p => p.ServicoId == lancheId);
        }

        public IEnumerable<Servico> GetServicosMaisVendidos(int quantidade)
        {
            var grupo = _context.PedidoDetalhes
                                        .Include(pd => pd.Servico)
                                        .AsEnumerable()
                                        .Where(pd => pd.Servico != null && pd.Servico.EmDisposicao)
                                        .GroupBy(pd => pd.Servico)
                                        .OrderByDescending(g => g.Sum(pd => pd.Quantidade))
                                        .Take(quantidade)
                                        .Select(g => g.Key)
                                        .ToList();

            return grupo;
        }



    }
}
