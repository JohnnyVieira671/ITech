using ITech.Context;
using ITech.Models;
using Microsoft.EntityFrameworkCore;

namespace ITech.Areas.Tecnicos.Services
{
    public class TecnicosRelatorioServicosService
    {
        private readonly AppDbContext _context;

        public TecnicosRelatorioServicosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Servico>> GetServicosReport()
        {
            var servicos = await _context.Servicos.ToListAsync();
            if (servicos is null)
                return default(IEnumerable<Servico>);

            return servicos;
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasReport()
        {
            var categorias = await _context.Categorias.ToListAsync();
            if (categorias is null)
                return default(IEnumerable<Categoria>);

            return categorias;
        }
    }
}
