using ITech.Models;

namespace ITech.ViewModels
{
    public class ServicoListViewModel
    {
        public IEnumerable<Servico> Servicos { get; set; }
        public string CategoriaAtual { get; set; }
    }
}
