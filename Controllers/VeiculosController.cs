using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Extensions;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class VeiculosController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search = null,
            int? situacao = null,
            string? orderBy = "Marca",
            string? orderDirection = "asc",
            int pageSize = 50,
            int page = 1)
        {
            var query = _context.Veiculos
                .Include(v => v.Proprietario)
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v =>
                    v.Marca.Descricao.Contains(search) ||
                    v.Modelo.Descricao.Contains(search) ||
                    v.Placa.ToLower().Contains(search.ToLower()) ||
                    v.Codigo.Contains(search) ||
                    v.Proprietario != null && v.Proprietario.Nome.Contains(search));
            }

            if (situacao.HasValue)
            {
                query = query.Where(v => (int)v.Situacao == situacao.Value);
            }

            // Aplicar ordenação
            query = orderBy?.ToLower() switch
            {
                "mara" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Marca).ThenBy(v => v.Modelo)
                    : query.OrderBy(v => v.Marca).ThenBy(v => v.Modelo),
                "preco" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.PrecoVenda)
                    : query.OrderBy(v => v.PrecoVenda),
                "situacao" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Situacao)
                    : query.OrderBy(v => v.Situacao),
                "datacadastro" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.DataCadastro)
                    : query.OrderBy(v => v.DataCadastro),
                _ => query.OrderBy(v => v.Marca).ThenBy(v => v.Modelo)
            };

            // Obter total de registros
            var totalRecords = await query.CountAsync();

            // Aplicar paginação
            List<Veiculo> veiculos;
            if (pageSize == -1) // "Todos"
            {
                veiculos = await query.ToListAsync();
            }
            else
            {
                veiculos = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            var viewModel = new VeiculosIndexViewModel
            {
                ListaObjeto = veiculos,
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                Search = search,
                Situacao = situacao != null ? (EnumSituacaoVeiculo)situacao : null,
                OrderBy = orderBy,
                OrderDirection = orderDirection,
                TotalPages = pageSize == -1 ? 1 : (int)Math.Ceiling((double)totalRecords / pageSize)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetVeiculosAjax(
            string? search = null,
            int? situacao = null,
            string? orderBy = "Marca",
            string? orderDirection = "asc",
            int pageSize = 50,
            int page = 1)
        {
            var result = await Index(search, situacao, orderBy, orderDirection, pageSize, page);

            return result is ViewResult viewResult && viewResult.Model is VeiculosIndexViewModel model
                ? PartialView("_VeiculosGrid", model)
                : (IActionResult)BadRequest();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .Include(v => v.Proprietario)
                .Include(v => v.Fotos)
                .Include(v => v.Documentos)
                .Include(v => v.Despesas)
                .FirstOrDefaultAsync(m => m.Id == id);

            return veiculo == null 
                ? NotFound()
                : View(veiculo);
        }

        public IActionResult Create()
        {
            ViewData["ProprietarioId"] = new SelectList(_context.Clientes, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Veiculo veiculo)
        {
            if (ModelState.IsValid)
            {
                veiculo.DataCadastro = DateTime.Now;
                _context.Add(veiculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProprietarioId"] = new SelectList(_context.Clientes, "Id", "Nome", veiculo.ProprietarioId);
            return View(veiculo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            ViewData["ProprietarioId"] = new SelectList(_context.Clientes, "Id", "Nome", veiculo.ProprietarioId);
            return View(veiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProprietarioId"] = new SelectList(_context.Clientes, "Id", "Nome", veiculo.ProprietarioId);
            return View(veiculo);
        }

        private bool VeiculoExists(int id)
        {
            return _context.Veiculos.Any(e => e.Id == id);
        }
    }
}