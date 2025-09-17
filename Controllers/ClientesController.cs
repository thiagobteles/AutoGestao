using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class ClientesController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search = null,
            EnumTipoPessoa? tipoCliente = null,
            bool? status = null,
            string? orderBy = "Nome",
            string? orderDirection = "asc",
            int pageSize = 50,
            int page = 1)
        {
            var query = _context.Clientes.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.Nome.Contains(search) ||
                    c.CPF != null && c.CPF.Contains(search) ||
                    c.CNPJ != null && c.CNPJ.Contains(search) ||
                    c.Email != null && c.Email.Contains(search) ||
                    c.Telefone != null && c.Telefone.Contains(search) ||
                    c.Celular != null && c.Celular.Contains(search));
            }

            if (tipoCliente != null)
            {
                query = query.Where(c => c.TipoCliente == tipoCliente);
            }

            if (status != null)
            {
                query = query.Where(c => c.Ativo == status);
            }

            // Aplicar ordenação
            query = orderBy?.ToLower() switch
            {
                "nome" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Nome)
                    : query.OrderBy(c => c.Nome),
                "tipo" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.TipoCliente)
                    : query.OrderBy(c => c.TipoCliente),
                "documento" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.CPF ?? c.CNPJ)
                    : query.OrderBy(c => c.CPF ?? c.CNPJ),
                "cidade" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Cidade)
                    : query.OrderBy(c => c.Cidade),
                "datacadastro" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.DataCadastro)
                    : query.OrderBy(c => c.DataCadastro),
                _ => query.OrderBy(c => c.Nome)
            };

            // Obter total de registros
            var totalRecords = await query.CountAsync();

            // Aplicar paginação
            List<Cliente> clientes;
            if (pageSize == -1) // "Todos"
            {
                clientes = await query.ToListAsync();
            }
            else
            {
                clientes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            var viewModel = new ClientesIndexViewModel
            {
                ListaObjeto = clientes,
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                Search = search,
                TipoCliente = tipoCliente,
                Ativo = status,
                OrderBy = orderBy,
                OrderDirection = orderDirection,
                TotalPages = pageSize == -1 ? 1 : (int)Math.Ceiling((double)totalRecords / pageSize)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetClientesAjax(
            string? search = null,
            EnumTipoPessoa? tipoCliente = null,
            bool? status = null,
            string? orderBy = "Nome",
            string? orderDirection = "asc",
            int pageSize = 50,
            int page = 1)
        {
            var result = await Index(search, tipoCliente, status, orderBy, orderDirection, pageSize, page);

            if (result is ViewResult viewResult && viewResult.Model is ClientesIndexViewModel model)
            {
                return PartialView("_ClientesGrid", model);
            }

            return BadRequest();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Veiculos)
                .Include(c => c.Vendas)
                .Include(c => c.Avaliacoes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.DataCadastro = DateTime.Now;
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}