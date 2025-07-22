using ITech.Models;

namespace ITech.Repositories.Interfaces
{
    public interface ITecnicoRepository
    {
        IEnumerable<Tecnico> Tecnicos { get; }
        Tecnico BuscarPorId(int id);
    }
}
