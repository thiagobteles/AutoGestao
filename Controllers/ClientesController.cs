using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class ClientesController(ApplicationDbContext context) : StandardGridController<Cliente>(context)
    {
        protected override IQueryable<Cliente> GetBaseQuery()
        {
            return _context.Clientes.AsQueryable();
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            return new StandardGridViewModel
            {
                Title = "Clientes",
                SubTitle = "",
                EntityName = "Clientes",
                ControllerName = "Clientes",

                // Configuração das colunas
                Columns =
                [
                    new() { Name = nameof(Cliente.Id), DisplayName = "Cód", Type = GridColumnType.Text, Sortable = true},
                    new() { Name = nameof(Cliente.TipoCliente), DisplayName = "Pessoa", Type = GridColumnType.Badge, Sortable = true },
                    new() { Name = nameof(Cliente.Nome), DisplayName = "Nome Completo", Sortable = true },
                    new() { Name = nameof(Cliente.CPF), DisplayName = "CPF/CNPJ", Sortable = true },
                    new() { Name = nameof(Cliente.Celular), DisplayName = "Telefone", Sortable = true },
                    new() { Name = nameof(Cliente.Ativo), DisplayName = "Status", Type = GridColumnType.Badge, Sortable = true },
                    new() { Name = "Actions", DisplayName = "Ações", Type = GridColumnType.Actions, Sortable = false }
                ],

                // Configuração dos filtros
                Filters =
                [
                    new()
                    {
                        Name = "search",
                        DisplayName = "Buscar",
                        Type = GridFilterType.Text,
                        Placeholder = "Busque por Nome, CPF, Telefone, Email..."
                    },
                    new()
                    {
                        Name = "coodigo",
                        DisplayName = "Codigo",
                        Type = GridFilterType.Number,
                        Placeholder = "Informe o código..."
                    },
                    new()
                    {
                        Name = "status",
                        DisplayName = "Status - Todos",
                        Type = GridFilterType.Select,
                        Options =
                        [
                            new() { Value = "", Text = "Status - Todos", Selected = true },
                            new() { Value = "true", Text = "Ativo" },
                            new() { Value = "false", Text = "Inativo" }
                        ]
                    }
                ],

                // Ações do cabeçalho
                HeaderActions =
                [
                    new()
                    {
                        Name = "Create",
                        DisplayName = "Novo",
                        Icon = "fas fa-plus",
                        CssClass = "btn-new",
                        Url = Url.Action("Create", "Clientes")
                    }
                ],

                // Ações das linhas
                RowActions =
                [
                    new()
                    {
                        Name = "Details",
                        DisplayName = "Visualizar",
                        Icon = "fas fa-eye",
                        Url = "/Clientes/Details/{id}"
                    },
                    new()
                    {
                        Name = "Edit",
                        DisplayName = "Alterar",
                        Icon = "fas fa-edit",
                        Url = "/Clientes/Edit/{id}"
                    },
                    new()
                    {
                        Name = "ToggleStatus",
                        DisplayName = "Inativar",
                        Icon = "fas fa-ban",
                        Url = "/Clientes/ToggleStatus/{id}",
                        ShowCondition = (x) => ((Cliente)x).Ativo == true
                    },
                    new()
                    {
                        Name = "ToggleStatus",
                        DisplayName = "Ativar",
                        Icon = "fas fa-check",
                        Url = "/Clientes/ToggleStatus/{id}",
                        ShowCondition = (item) => ((Cliente)item).Ativo == false
                    }
                ]
            };
        }

        protected override IQueryable<Cliente> ApplyFilters(IQueryable<Cliente> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = query.Where(c =>
                                c.Nome.Contains(searchTerm) ||
                                (c.CPF != null && c.CPF.Contains(searchTerm)) ||
                                (c.CNPJ != null && c.CNPJ.Contains(searchTerm)) ||
                                (c.Email != null && c.Email.Contains(searchTerm)) ||
                                (c.Telefone != null && c.Telefone.Contains(searchTerm)) ||
                                (c.Celular != null && c.Celular.Contains(searchTerm)));
                        }
                        break;

                    case "status":
                        if (bool.TryParse(filter.Value.ToString(), out bool status))
                        {
                            query = query.Where(c => c.Ativo == status);
                        }
                        break;

                    case "tipocliente":
                        if (Enum.TryParse<EnumTipoPessoa>(filter.Value.ToString(), out EnumTipoPessoa tipoCliente))
                        {
                            query = query.Where(c => c.TipoCliente == tipoCliente);
                        }
                        break;

                    case "estado":
                        if (Enum.TryParse<EnumEstado>(filter.Value.ToString(), out EnumEstado estado))
                        {
                            query = query.Where(c => c.Estado == estado);
                        }
                        break;

                    case "cidade":
                        var cidade = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(cidade))
                        {
                            query = query.Where(c => c.Cidade != null && c.Cidade.Contains(cidade));
                        }
                        break;

                    case "datacadastro":
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dataCadastro))
                        {
                            query = query.Where(c => c.DataCadastro.Date == dataCadastro.Date);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<Cliente> ApplySort(IQueryable<Cliente> query, string orderBy, string orderDirection)
        {
            return orderBy?.ToLower() switch
            {
                "nome" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Nome)
                    : query.OrderBy(c => c.Nome),
                "tipocliente" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.TipoCliente)
                    : query.OrderBy(c => c.TipoCliente),
                "datacadastro" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.DataCadastro)
                    : query.OrderBy(c => c.DataCadastro),
                "cidade" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Cidade)
                    : query.OrderBy(c => c.Cidade),
                _ => query.OrderByDescending(c => c.DataCadastro) // Default: últimos cadastros
            };
        }

        // Ações específicas
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Ativo = !cliente.Ativo;
                cliente.DataAlteracao = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
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

            return cliente == null ? NotFound() : View(cliente);
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
                cliente.DataAlteracao = DateTime.Now;
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
            return cliente == null 
                ? NotFound()
                : View(cliente);
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
                    cliente.DataAlteracao = DateTime.Now;
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

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}