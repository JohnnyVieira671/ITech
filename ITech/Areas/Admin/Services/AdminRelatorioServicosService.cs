using ITech.Context;
using ITech.Models;
using Microsoft.EntityFrameworkCore;

namespace ITech.Areas.Admin.Services
{
    public class AdminRelatorioServicosService
    {
        private readonly AppDbContext _context;

        public AdminRelatorioServicosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Servico>> GetLanchesReport()
        {
            var lanches = await _context.Servicos.ToListAsync();
            if (lanches is null)
                return default(IEnumerable<Servico>);

            return lanches;
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
