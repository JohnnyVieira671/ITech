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

namespace ITech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminPedidos
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            var resultado = _context.Pedidos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                resultado = resultado.Where(p => p.Nome.Contains(filter) ||
                                                 p.Sobrenome.Contains(filter) ||
                                                 p.Endereco1.Contains(filter) ||
                                                 p.Endereco2.Contains(filter) ||
                                                 p.Cep.Contains(filter) ||
                                                 p.Estado.Contains(filter) ||
                                                 p.Cidade.Contains(filter) ||
                                                 p.Telefone.Contains(filter) ||
                                                 p.Email.Contains(filter));
            }

            var model = await PagingList.CreateAsync(resultado, 3, pageindex, sort, "Nome");
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

        // GET: Admin/AdminPedidos/Create
        public IActionResult Create()
        {
            ViewBag.Servicos = _context.Servicos.ToList();
            return View(new Pedido());
        }

        // POST: Admin/AdminPedidos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pedido pedido, int[] servicoIds, int[] quantidades)
        {
            if (ModelState.IsValid)
            {
                // Verifica se pelo menos um item tem quantidade > 0
                bool temItensValidos = quantidades != null && quantidades.Any(q => q > 0);

                if (!temItensValidos)
                {
                    ModelState.AddModelError("", "Você deve adicionar pelo menos um item ao pedido com quantidade maior que zero.");
                }
                else
                {
                    pedido.PedidoEnviado = DateTime.Now;

                    _context.Pedidos.Add(pedido);
                    await _context.SaveChangesAsync();

                    pedido.PedidoItens = new List<PedidoDetalhe>();

                    for (int i = 0; i < servicoIds.Length; i++)
                    {
                        if (quantidades[i] > 0)
                        {
                            var servico = await _context.Servicos.FindAsync(servicoIds[i]);
                            pedido.PedidoItens.Add(new PedidoDetalhe
                            {
                                PedidoId = pedido.PedidoId,
                                ServicoId = servico.ServicoId,
                                Quantidade = quantidades[i],
                                Preco = servico.Valor
                            });
                        }
                    }

                    _context.Pedidos.Update(pedido);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Servicos = _context.Servicos.ToList();
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

                    return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
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
