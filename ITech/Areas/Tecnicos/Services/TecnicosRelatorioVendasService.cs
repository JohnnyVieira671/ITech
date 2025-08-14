using ITech.Context;
using ITech.Models;
using Microsoft.EntityFrameworkCore;

namespace ITech.Areas.Tecnicos.Services
{
    public class TecnicosRelatorioVendasService
    {
        private readonly AppDbContext _context;
        public TecnicosRelatorioVendasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pedido>> FindByDateAsync(DateTime? minDate, DateTime? maxDate, string email)
        {
            var resultado = from obj in _context.Pedidos select obj;

            if (minDate.HasValue)
            {
                resultado = resultado.Where(x => x.PedidoEnviado>=minDate.Value);
            }
            if (maxDate.HasValue)
            {
                resultado = resultado.Where(x => x.PedidoEnviado <= maxDate.Value);
            }
            return await _context.Pedidos
                            .Where(p => p.PedidoItens.Any(i => i.Servico.Tecnicos.Email == email)) 
                            .Include(p => p.PedidoItens.Where(i => i.Servico.Tecnicos.Email == email)) 
                                  .ThenInclude(i => i.Servico)
                            .Where(p => (!minDate.HasValue || p.PedidoEnviado >= minDate) &&
                                  (!maxDate.HasValue || p.PedidoEnviado <= maxDate))
                            .OrderByDescending(p => p.PedidoEnviado)
                            .ToListAsync();


        }


        public async Task<List<Pedido>> FindByFilterAsync(string filter, string email)
        {
            var resultado = from obj in _context.Pedidos select obj;
                return await _context.Pedidos
                            .Where(p => p.PedidoItens.Any(i => i.Servico.Tecnicos.Email == email))
                            .Include(p => p.PedidoItens.Where(i => i.Servico.Tecnicos.Email == email))
                                  .ThenInclude(i => i.Servico)
                            .Where(p => (
                                                p.Nome.Contains(filter) ||
                                                p.Sobrenome.Contains(filter) ||
                                                p.Endereco1.Contains(filter) ||
                                                p.Endereco2.Contains(filter) ||
                                                p.Cep.Contains(filter) ||
                                                p.Estado.Contains(filter) ||
                                                p.Cidade.Contains(filter) ||
                                                p.Telefone.Contains(filter) ||
                                                p.Email.Contains(filter)
                                        )
                                    )
                            .OrderByDescending(p => p.PedidoEnviado)
                            .ToListAsync();
        }

    }
}
