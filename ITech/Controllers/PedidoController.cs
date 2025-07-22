using ITech.Models;
using ITech.Repositories;
using ITech.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

using System.Diagnostics.CodeAnalysis;

namespace ITech.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ITecnicoRepository _tecnicoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoController(IPedidoRepository pedidoRepository, CarrinhoCompra carrinhoCompra, ITecnicoRepository tecnicoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoCompra = carrinhoCompra;
            _tecnicoRepository = tecnicoRepository;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(Pedido pedido)
        {
            int totalItensPedido = 0;
            decimal precoTotalPedido = 0.0m;

            //Obtém itens do carrinho de compra do cliente

            List<CarrinhoCompraItem> itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItens = itens;

            if (_carrinhoCompra.CarrinhoCompraItens.Count == 0)
            {
                ModelState.AddModelError("", "Seu carrinho está vazio, que tal adicionar um serviço de TI...");
            }

            foreach (var item in itens)
            {
                totalItensPedido += item.Quantidade;
                precoTotalPedido += (item.Servico.Valor * item.Quantidade);


                //Envia um email para o tecnico avisando da venda
                var tecnicoId = item.Servico.TecnicoId;
                var tecnico = _tecnicoRepository.BuscarPorId(tecnicoId);

                var mensagem = $@"
                    Prezado(a) {tecnico.TecnicoNome},

                    Você foi contratado para prestar o serviço '{item.Servico.DescricaoCurta}' através da plataforma ITech.

                    Quantidade contratada: {item.Quantidade}
                    Valor total do serviço: {(item.Servico.Valor * item.Quantidade).ToString("C")}

                    Para prosseguir com os próximos passos, entre em contato com o cliente:
                    E-mail do cliente: {pedido.Email}
                    Telefone do cliente: {pedido.Telefone}

                    Atenciosamente,  
                    Equipe de Atendimento ITech!!
                    ";

                var mail = new MailMessage();
                mail.To.Add(tecnico.Email);
                mail.From = new MailAddress("itech.technology80@gmail.com");
                mail.Subject = "Novo pedido atribuído a você";
                mail.Body = mensagem;

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("itech.technology80@gmail.com", "katpwsmitblzxxqc");
                    smtp.EnableSsl = true;

                    smtp.Send(mail);
                }
            }

            pedido.TotalItensPedido = totalItensPedido;
            pedido.PedidoTotal = precoTotalPedido;

            if (ModelState.IsValid)
            {
                _pedidoRepository.CriarPedido(pedido);

                ViewBag.CheckoutCompletoMensagem = "Obrigado pelo seu pedido!";
                ViewBag.TotalPedido = _carrinhoCompra.GetCarrinhoCompraTotal();

                _carrinhoCompra.LimparCarrinho();

                return View("~/Views/Pedido/CheckoutCompleto.cshtml", pedido);

            }
            return View(pedido);
        }

    }
}
