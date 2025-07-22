using ITech.Context;
using ITech.Models;
using Microsoft.EntityFrameworkCore;

namespace ITech.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _context;

        public CarrinhoCompra(AppDbContext context)
        {
            _context = context;
        }

        public string CarrinhoCompraId { get; set; }
        public List<CarrinhoCompraItem> CarrinhoCompraItens { get; set; }

        public static CarrinhoCompra GetCarrinho(IServiceProvider services)
        {
            //define uma sessão
            ISession session =
                services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            //obtem o serviço do tipo do nosso contexto
            var context = services.GetService<AppDbContext>();

            //obtem ou gera o id do carrinho
            string carrinhoId = session.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();

            session.SetString("CarrinhoId", carrinhoId);

            return new CarrinhoCompra(context)
            {
                CarrinhoCompraId = carrinhoId
            };
        }

        public void AdicionarAoCarrinho(Servico servico)
        {
            // Verifica se o lanche é nulo
            var carrinhoCompraItem = _context.CarrinhoCompraItens.SingleOrDefault(
                c => c.Servico.ServicoId == servico.ServicoId &&
                c.CarrinhoCompraId == CarrinhoCompraId);

            // Verifica se o item já existe no carrinho
            if (carrinhoCompraItem == null)
            {
                carrinhoCompraItem = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId,
                    Servico = servico,
                    Quantidade = 1
                };
                _context.CarrinhoCompraItens.Add(carrinhoCompraItem);
            }
            else
            {
                carrinhoCompraItem.Quantidade++;
            }
            _context.SaveChanges();
        }

        public int RemoverDoCarrinho(Servico Servico)
        {
            var carrinhoCompraItem = _context.CarrinhoCompraItens.SingleOrDefault(
                c => c.Servico.ServicoId == Servico.ServicoId &&
                c.CarrinhoCompraId == CarrinhoCompraId);

            var quantidadeLocal = 0;

            if (carrinhoCompraItem != null)
            {
                _context.CarrinhoCompraItens.Remove(carrinhoCompraItem);

            }
            _context.SaveChanges();
            return quantidadeLocal;
        }

        public List<CarrinhoCompraItem> GetCarrinhoCompraItens()
        {
            return CarrinhoCompraItens ?? (CarrinhoCompraItens =
                _context.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                .Include(s => s.Servico)
                .ToList());
        }

        public void LimparCarrinho()
        {
            var carrinhoItens = _context.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId);
            _context.CarrinhoCompraItens.RemoveRange(carrinhoItens);
            _context.SaveChanges();
        }

        public decimal GetCarrinhoCompraTotal()
        {
            var total = _context.CarrinhoCompraItens
                .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                .Select(c=> c.Servico.Valor * c.Quantidade).Sum();

            return total;
        }

        public void AtualizarQuantidade(Servico servico, int novaQuantidade)
        {
            var item = _context.CarrinhoCompraItens
                .SingleOrDefault(s => s.Servico.ServicoId == servico.ServicoId && s.CarrinhoCompraId == CarrinhoCompraId);

            if (item != null)
            {
                item.Quantidade = novaQuantidade;
                _context.SaveChanges();
            }
        }
    }
}
