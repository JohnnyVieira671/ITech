using ITech.Context;
using ITech.Models;
using ITech.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System.Net.Mail;
using System.Net;

namespace ITech.Controllers
{
    public class ManipulaPedidosController : Controller
    {
        private readonly AppDbContext _context;

        public ManipulaPedidosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "Nome")
        {
            var email = User.Identity?.Name;

            var pedido = _context.Pedidos
                                        .Include(p => p.PedidoItens)
                                            .ThenInclude(p => p.Servico);

            List<Pedido> resultado;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                 resultado = await _context.Pedidos
                            .Where(p => p.Email == email)
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
            else
            {
                 resultado = await _context.Pedidos
                            .Where(p => p.Email == email)
                            .Include(p => p.PedidoItens.Where(i => i.Servico.Tecnicos.Email == email))
                                  .ThenInclude(i => i.Servico)
                            .OrderByDescending(p => p.PedidoEnviado)
                            .ToListAsync(); ;
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
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Pedidos == null)
        //    {
        //        return NotFound();
        //    }

        //    var pedido = await _context.Pedidos
        //        .Include(p => p.PedidoItens)
        //        .ThenInclude(pi => pi.Servico)
        //        .FirstOrDefaultAsync(p => p.PedidoId == id);

        //    if (pedido == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(pedido);
        //}

        // POST: Admin/AdminPedidos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEntrega(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
                return NotFound();

            if (pedido.PedidoEntregueEm != null)
            {
                // Pedido já confirmado
                return BadRequest("Este pedido já foi confirmado como entregue.");
            }

            pedido.PedidoEntregueEm = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ManipulaPedidos", new { area = (string)null });
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

            var email = User.Identity?.Name;

            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                    .ThenInclude(pi => pi.Servico)
                        .ThenInclude(s => s.Tecnicos)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null)
            {
                return NotFound();
            }else
            {
                if (pedido.PedidoEntregueEm != null && pedido.PedidoEnviado <= DateTime.Now.AddDays(-15))
                {
                    _context.Pedidos.Remove(pedido);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ManipulaPedidos", new { area = (string)null });
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
                    .ThenInclude(s => s.Tecnicos)
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
