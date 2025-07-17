using ITech.Models;
using ITech.Repositories.Interfaces;
using ITech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ITech.Controllers
{
    public class ServicoController : Controller
    {
        private readonly IServicoRepository _servicoRepository;

        public ServicoController(IServicoRepository servicoRepository)
        {
            _servicoRepository = servicoRepository;
        }

        public IActionResult List(string categoria)
        {
            IEnumerable<Servico> servicos;
            string categoriaAtual = string.Empty;

            if (string.IsNullOrEmpty(categoria))
            {
                servicos = _servicoRepository.Servicos
                            .Where(s => s.EmDisposicao)
                            .OrderBy(s => s.ServicoId);
                categoriaAtual = "Todos os Serviços";
            }
            else
            {
                servicos = _servicoRepository.Servicos
                        .Where(s => s.Categoria.CategoriaNome.ToLower() == categoria.ToLower() && s.EmDisposicao)
                        .OrderBy(s => s.ServicoId);

                categoriaAtual = categoria;
            }

            var servicoListViewModel = new ServicoListViewModel
            {
                Servicos = servicos,
                CategoriaAtual = categoriaAtual
            };

            return View(servicoListViewModel); 
        }

        public IActionResult Details(int servicoId)
        {
            var servico = _servicoRepository.Servicos.FirstOrDefault(s => s.ServicoId == servicoId);
            return View(servico);
        }

        public ViewResult Search(string searchString)
        {
            IEnumerable<Servico> servicos;
            string categoriaAtual = string.Empty;

            if (string.IsNullOrEmpty(searchString))
            {
                servicos = _servicoRepository.Servicos
                            .Where(s=> s.EmDisposicao)
                            .OrderBy(s => s.ServicoId);
                categoriaAtual = "Todos os serviços";
            }
            else
            {
                servicos = _servicoRepository.Servicos
                    .Where(s => s.DescricaoCurta.ToLower().Contains(searchString.ToLower()) && s.EmDisposicao)
                    .OrderBy(s => s.ServicoId);

                categoriaAtual = servicos.Any() ? "Serviço" : "Nenhum Serviço foi encontrado";
            }

            var viewModel = new ServicoListViewModel
            {
                Servicos = servicos,
                CategoriaAtual = categoriaAtual
            };

            return View("~/Views/Servico/List.cshtml", viewModel);

        }
    }
}
