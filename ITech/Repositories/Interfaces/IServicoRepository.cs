using ITech.Models;

namespace ITech.Repositories.Interfaces
{
    public interface IServicoRepository
    {
        IQueryable<Servico> Servicos { get; }

        //IEnumerable<Servico> ServicosPreferidos { get; }

        IEnumerable<Servico> GetServicosMaisVendidos(int quantidade);

        Servico GetlancheById(int lancheId);
    }
}
