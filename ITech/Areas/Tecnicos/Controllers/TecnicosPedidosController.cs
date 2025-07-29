using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITech.Context;
using ITech.Models;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using ITech.ViewModels;
using ITech.Areas.Tecnicos.Services;

namespace ITech.Areas.Tecnicos.Controllers
{
    [Area("Tecnicos")]
    [Authorize(Roles = "Tecnicos")]
    public class TecnicosPedidosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TecnicosRelatorioVendasService _relatorioVendasService;

        public TecnicosPedidosController(AppDbContext context, TecnicosRelatorioVendasService relatorioVendasService)
        {
            _relatorioVendasService = relatorioVendasService;
            _context = context;
        }

        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            var email = User.Identity?.Name;

            List<Pedido> resultado;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = await _relatorioVendasService.FindByFilterAsync(filter, email);
            }
            else
            {
                resultado = await _relatorioVendasService.FindByDateAsync(null, null, email);
            }

            var model = PagingList.Create(resultado, 5, pageindex, sort, "Nome"); 

            model.RouteValue = new RouteValueDictionary { { "filter", filter } };

            return View(model);
        }


        // GET: Admin/AdminPedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                .ThenInclude(pi => pi.Servico)
                .FirstOrDefaultAsync(m => m.PedidoId == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                .ThenInclude(pi => pi.Servico)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Admin/AdminPedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pedido pedido, List<PedidoDetalhe> PedidoItens)
        {
            if (id != pedido.PedidoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var pedidoDb = await _context.Pedidos
                        .Include(p => p.PedidoItens)
                        .FirstOrDefaultAsync(p => p.PedidoId == id);

                    if (pedidoDb == null)
                        return NotFound();

                    // Atualiza os dados do pedido
                    pedidoDb.Nome = pedido.Nome;
                    pedidoDb.Sobrenome = pedido.Sobrenome;
                    pedidoDb.Endereco1 = pedido.Endereco1;
                    pedidoDb.Endereco2 = pedido.Endereco2;
                    pedidoDb.Cep = pedido.Cep;
                    pedidoDb.Estado = pedido.Estado;
                    pedidoDb.Cidade = pedido.Cidade;
                    pedidoDb.Telefone = pedido.Telefone;
                    pedidoDb.Email = pedido.Email;
                    pedidoDb.PedidoEnviado = pedido.PedidoEnviado;
                    pedidoDb.PedidoEntregueEm = pedido.PedidoEntregueEm;

                    // Atualiza as quantidades dos itens
                    foreach (var itemAtual in pedidoDb.PedidoItens.ToList())
                    {
                        var itemEditado = PedidoItens.FirstOrDefault(pi => pi.PedidoDetalheId == itemAtual.PedidoDetalheId);
                        if (itemEditado != null)
                        {
                            if (itemEditado.Quantidade <= 0)
                            {
                                _context.PedidoDetalhes.Remove(itemAtual);
                            }
                            else
                            {
                                itemAtual.Quantidade = itemEditado.Quantidade;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "TecnicosPedidos", new { area = "Tecnicos" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.PedidoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(pedido);
        }

        // GET: Admin/AdminPedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedidos == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.PedidoId == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Admin/AdminPedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedidos == null)
            {
                return Problem("Entity set 'AppDbContext.Pedidos' is null.");
            }
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "TecnicosPedidos", new { area = "Tecnicos" });
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.PedidoId == id);
        }

        // Visualizar os itens do pedido usando PedidoServicoViewModel
        public IActionResult PedidoServicos(int? id)
        {
            if (id == null)
                return NotFound();

            var pedido = _context.Pedidos
                .Include(p => p.PedidoItens)
                .ThenInclude(pi => pi.Servico)
                .FirstOrDefault(p => p.PedidoId == id);

            if (pedido == null)
            {
                Response.StatusCode = 404;
                return View("PedidoNotFound", id.Value);
            }

            var pedidoServicos = new PedidoServicoViewModel
            {
                Pedido = pedido,
                PedidoDetalhes = pedido.PedidoItens
            };

            return View(pedidoServicos);
        }
    }
}
