using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Extensions;
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
            var retorno = new StandardGridViewModel("Clientes", "Gerencie todos os clientes do sistema", "Clientes")
            {
                Filters =
                    [
                        new()
                        {
                            Name = "search",
                            DisplayName = "Busca Geral",
                            Type = GridFilterType.Text,
                            Placeholder = "Nome, CPF, CNPJ, Email, Telefone..."
                        },
                        new()
                        {
                            Name = "tipocliente",
                            DisplayName = "Tipo de Pessoa",
                            Type = GridFilterType.Select,
                            Placeholder = "Tipo Pessoa...",
                            Options = GetTipoClienteOptions()
                        },
                        new()
                        {
                            Name = "status",
                            DisplayName = "Status",
                            Type = GridFilterType.Select,
                            Placeholder = "Status cliente...",
                            Options =
                            [
                                new() { Value = "true", Text = "✅ Ativo" },
                                new() { Value = "false", Text = "❌ Inativo" }
                            ]
                        }
                    ],

                Columns =
                    [
                        new() { Name = nameof(Cliente.Id), DisplayName = "Cód", Type = GridColumnType.Text, Sortable = true, Width = "80px"},
                        new() { Name = nameof(Cliente.TipoCliente), DisplayName = "Tipo", Type = GridColumnType.Badge, Sortable = true, Width = "60px" },
                        new() { Name = nameof(Cliente.Nome), DisplayName = "Nome/Razão Social", Sortable = true },
                        new() { Name = "Documento", DisplayName = "CPF/CNPJ", Sortable = true, Type = GridColumnType.Custom, CustomRender = RenderDocumento},
                        new() { Name = nameof(Cliente.Celular), DisplayName = "Telefone", Sortable = true, Width = "130px" },
                        new() { Name = nameof(Cliente.Cidade), DisplayName = "Cidade", Sortable = true, Width = "150px" },
                        new() { Name = nameof(Cliente.Estado), DisplayName = "UF", Type = GridColumnType.Badge, Sortable = true, Width = "60px" },
                        new() { Name = nameof(Cliente.Ativo), DisplayName = "Status", Type = GridColumnType.Badge, Sortable = true, Width = "100px" },
                        new() { Name = "Actions", DisplayName = "Ações", Type = GridColumnType.Actions, Sortable = false, Width = "120px" }
                    ]
            };

            retorno.RowActions.AddRange(
                [
                    new()
                    {
                        Name = "NewSale",
                        DisplayName = "Nova Venda",
                        Icon = "fas fa-handshake",
                        Url = "/Vendas/Create?clienteId={id}",
                        ShowCondition = (x) => ((Cliente)x).Ativo == true
                    },
                    new()
                    {
                        Name = "NewEvaluation",
                        DisplayName = "Nova Avaliação",
                        Icon = "fas fa-clipboard-check",
                        Url = "/Avaliacoes/Create?clienteId={id}",
                        ShowCondition = (x) => ((Cliente)x).Ativo == true
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
                ]);

            return retorno;
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
                            query = ApplyTextFilter(query, searchTerm,
                                c => c.Nome,
                                c => c.CPF,
                                c => c.CNPJ,
                                c => c.Email,
                                c => c.Telefone,
                                c => c.Celular);
                        }
                        break;

                    case "codigo":
                        if (int.TryParse(filter.Value.ToString(), out int codigo))
                        {
                            query = query.Where(c => c.Id == codigo);
                        }
                        break;

                    case "status":
                        if (bool.TryParse(filter.Value.ToString(), out bool status))
                        {
                            query = query.Where(c => c.Ativo == status);
                        }
                        break;

                    case "tipocliente":
                        query = ApplyEnumFilter<EnumTipoPessoa>(query, filters, filter.Key, c => c.TipoCliente);
                        break;

                    case "estado":
                        query = ApplyEnumFilter<EnumEstado>(query, filters, filter.Key, c => c.Estado);
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

                    case "periodo_inicio":
                    case "periodo_fim":
                        // Será tratado pelo helper ApplyDateRangeFilter
                        break;
                }
            }

            // Aplicar filtro de período usando o helper
            query = ApplyDateRangeFilter(query, filters, "periodo", c => c.DataCadastro);

            return query;
        }

        protected override IQueryable<Cliente> ApplySort(IQueryable<Cliente> query, string orderBy, string orderDirection)
        {
            return orderBy?.ToLower() switch
            {
                "id" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id),
                "nome" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Nome)
                    : query.OrderBy(c => c.Nome),
                "tipocliente" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.TipoCliente)
                    : query.OrderBy(c => c.TipoCliente),
                "cpf" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.CPF)
                    : query.OrderBy(c => c.CPF),
                "cidade" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Cidade)
                    : query.OrderBy(c => c.Cidade),
                "estado" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Estado)
                    : query.OrderBy(c => c.Estado),
                "datacadastro" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.DataCadastro)
                    : query.OrderBy(c => c.DataCadastro),
                "ativo" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Ativo)
                    : query.OrderBy(c => c.Ativo),
                _ => query.OrderByDescending(c => c.DataCadastro) // Default: Últimos cadastros
            };
        }

        #region Métodos Auxiliares para Filtros

        private static List<SelectListItem> GetTipoClienteOptions()
        {
            var options = new List<SelectListItem>();
            var enumDictionary = EnumExtension.GetEnumDictionary<EnumTipoPessoa>();

            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                options.Add(new SelectListItem
                {
                    Value = ((EnumTipoPessoa)item.Key).ToString(),
                    Text = $"{GetTipoIcon((EnumTipoPessoa)item.Key)} {item.Value}"
                });
            }

            return options;
        }

        private static string GetTipoIcon(EnumTipoPessoa tipo)
        {
            return tipo switch
            {
                EnumTipoPessoa.PessoaFisica => "👤",
                EnumTipoPessoa.PessoaJuridica => "🏢",
                _ => "❓"
            };
        }

        private static string RenderDocumento(object item)
        {
            var cliente = (Cliente)item;
            var documento = cliente.TipoCliente == EnumTipoPessoa.PessoaJuridica 
                ? cliente.CNPJ.AplicarMascaraCnpj()
                : cliente.CPF.AplicarMascaraCpf();

            return $@"{documento}";
        }

        #endregion

        #region Ações Específicas

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Ativo = !cliente.Ativo;
                cliente.DataAlteracao = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Cliente {(cliente.Ativo ? "ativado" : "inativado")} com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Cliente não encontrado!";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Validações específicas
                if (cliente.TipoCliente == EnumTipoPessoa.PessoaFisica && string.IsNullOrEmpty(cliente.CPF))
                {
                    ModelState.AddModelError("CPF", "CPF é obrigatório para Pessoa Física");
                }

                if (cliente.TipoCliente == EnumTipoPessoa.PessoaJuridica && string.IsNullOrEmpty(cliente.CNPJ))
                {
                    ModelState.AddModelError("CNPJ", "CNPJ é obrigatório para Pessoa Jurídica");
                }

                // Verificar duplicidade de CPF/CNPJ
                if (!string.IsNullOrEmpty(cliente.CPF))
                {
                    var existeCPF = await _context.Clientes.AnyAsync(c => c.CPF == cliente.CPF);
                    if (existeCPF)
                    {
                        ModelState.AddModelError("CPF", "Já existe um cliente com este CPF");
                    }
                }

                if (!string.IsNullOrEmpty(cliente.CNPJ))
                {
                    var existeCNPJ = await _context.Clientes.AnyAsync(c => c.CNPJ == cliente.CNPJ);
                    if (existeCNPJ)
                    {
                        ModelState.AddModelError("CNPJ", "Já existe um cliente com este CNPJ");
                    }
                }

                if (ModelState.IsValid)
                {
                    cliente.DataCadastro = DateTime.Now;
                    cliente.DataAlteracao = DateTime.Now;
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cliente cadastrado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
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
                    // Validações específicas
                    if (cliente.TipoCliente == EnumTipoPessoa.PessoaFisica && string.IsNullOrEmpty(cliente.CPF))
                    {
                        ModelState.AddModelError("CPF", "CPF é obrigatório para Pessoa Física");
                    }

                    if (cliente.TipoCliente == EnumTipoPessoa.PessoaJuridica && string.IsNullOrEmpty(cliente.CNPJ))
                    {
                        ModelState.AddModelError("CNPJ", "CNPJ é obrigatório para Pessoa Jurídica");
                    }

                    // Verificar duplicidade de CPF/CNPJ (exceto o próprio registro)
                    if (!string.IsNullOrEmpty(cliente.CPF))
                    {
                        var existeCPF = await _context.Clientes.AnyAsync(c => c.CPF == cliente.CPF && c.Id != cliente.Id);
                        if (existeCPF)
                        {
                            ModelState.AddModelError("CPF", "Já existe outro cliente com este CPF");
                        }
                    }

                    if (!string.IsNullOrEmpty(cliente.CNPJ))
                    {
                        var existeCNPJ = await _context.Clientes.AnyAsync(c => c.CNPJ == cliente.CNPJ && c.Id != cliente.Id);
                        if (existeCNPJ)
                        {
                            ModelState.AddModelError("CNPJ", "Já existe outro cliente com este CNPJ");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        cliente.DataAlteracao = DateTime.Now;
                        _context.Update(cliente);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Cliente atualizado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
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

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Remove(cliente);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cliente deletado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(cliente);
        }

        public async Task<IActionResult> Delete(int? id)
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

            try
            {
                _context.Remove(cliente);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cliente deletado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            try
            {
                var clientes = await _context.Clientes
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("ID,Tipo,Nome,CPF/CNPJ,Email,Telefone,Celular,Cidade,Estado,Status,Data Cadastro");

                foreach (var cliente in clientes)
                {
                    var documento = !string.IsNullOrEmpty(cliente.CPF) ? cliente.CPF : cliente.CNPJ;
                    csv.AppendLine($"{cliente.Id}," +
                                  $"{cliente.TipoCliente.GetDescription()}," +
                                  $"\"{cliente.Nome}\"," +
                                  $"{documento}," +
                                  $"{cliente.Email}," +
                                  $"{cliente.Telefone}," +
                                  $"{cliente.Celular}," +
                                  $"{cliente.Cidade}," +
                                  $"{cliente.Estado.GetDescription()}," +
                                  $"{(cliente.Ativo ? "Ativo" : "Inativo")}," +
                                  $"{cliente.DataCadastro:dd/MM/yyyy}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"clientes_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao exportar dados: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            TempData["ErrorMessage"] = $"Operação ainda não implementada!";
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        #endregion
    }
}