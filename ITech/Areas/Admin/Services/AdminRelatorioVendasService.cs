﻿using ITech.Context;
using ITech.Models;
using Microsoft.EntityFrameworkCore;

namespace ITech.Areas.Admin.Services
{
    public class AdminRelatorioVendasService
    {
        private readonly AppDbContext _context;
        public AdminRelatorioVendasService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Pedido>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
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

            return await resultado
                             .Include(p => p.PedidoItens)
                             .ThenInclude(pi => pi.Servico)
                             .Where(p => p.PedidoItens.All(i => i.Servico != null)) // <- aqui
                             .OrderByDescending(x => x.PedidoEnviado)
                             .ToListAsync();

        }
        
    }
}
