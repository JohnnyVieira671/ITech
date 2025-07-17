using ITech.Context;
using ITech.Models;
using ITech.Repositories.Interfaces;

namespace ITech.Repositories
{
    public class TecnicoRepository: ITecnicoRepository
    {
        private readonly AppDbContext _context;

        public TecnicoRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Tecnico> Tecnicos => _context.Tecnicos;
    }
}
