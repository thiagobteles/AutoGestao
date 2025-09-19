using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Models;
using AutoGestao.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class VendedoresController(ApplicationDbContext context) : StandardGridController<Vendedor>(context)
    {
        #region Implementação Obrigatória (4 métodos apenas!)

        protected override IQueryable<Vendedor> GetBaseQuery()
        {
            // 1️⃣ Query base - apenas definir includes se necessário
            return _context.Vendedores.AsQueryable();
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            // 2️⃣ Configuração da grid - copiar e personalizar
            return new StandardGridViewModel
            {
                Title = "Vendedores",
                SubTitle = "Gerencie todos os vendedores do sistema",
                EntityName = "Vendedores",
                ControllerName = "Vendedores",

                // ✨ Colunas (5 minutos para configurar)
                Columns =
                [
                    new() { Name = nameof(Vendedor.Id), DisplayName = "Cód", Type = GridColumnType.Number, Sortable = true, Width = "70px"},
                    new() { Name = nameof(Vendedor.Nome), DisplayName = "Nome Completo", Sortable = true },
                    new() { Name = nameof(Vendedor.CPF), DisplayName = "CPF", Sortable = true, Width = "130px" },
                    new() { Name = nameof(Vendedor.Email), DisplayName = "Email", Sortable = true },
                    new() { Name = nameof(Vendedor.Celular), DisplayName = "Celular", Sortable = true, Width = "130px" },
                    new() { Name = nameof(Vendedor.PercentualComissao), DisplayName = "Comissão %", Type = GridColumnType.Number, Sortable = true, Width = "110px" },
                    new() { Name = nameof(Vendedor.Meta), DisplayName = "Meta", Type = GridColumnType.Currency, Sortable = true, Width = "120px" },
                    new() { Name = nameof(Vendedor.Ativo), DisplayName = "Status", Type = GridColumnType.Badge, Sortable = true, Width = "100px" },
                    new() { Name = "Actions", DisplayName = "Ações", Type = GridColumnType.Actions, Sortable = false, Width = "120px" }
                ],

                // 🔍 Filtros (3 minutos para configurar)
                Filters =
                [
                    new()
                    {
                        Name = "search",
                        DisplayName = "Busca Geral",
                        Type = GridFilterType.Text,
                        Placeholder = "Nome, CPF, Email..."
                    },
                    new()
                    {
                        Name = "status",
                        DisplayName = "Status",
                        Type = GridFilterType.Select,
                        Options =
                        [
                            new() { Value = "", Text = "Todos os Status", Selected = true },
                            new() { Value = "true", Text = "✅ Ativo" },
                            new() { Value = "false", Text = "❌ Inativo" }
                        ]
                    },
                    new()
                    {
                        Name = "meta_min",
                        DisplayName = "Meta Mínima",
                        Type = GridFilterType.Number,
                        Placeholder = "R$ 0,00"
                    }
                ],

                // 🎯 Ações do cabeçalho (1 minuto)
                HeaderActions =
                [
                    new()
                    {
                        Name = "Create",
                        DisplayName = "Novo Vendedor",
                        Icon = "fas fa-plus",
                        CssClass = "btn-new",
                        Url = Url.Action("Create", "Vendedores")
                    }
                ],

                // ⚡ Ações das linhas (2 minutos) - COM CONDIÇÕES!
                RowActions =
                [
                    new()
                    {
                        Name = "Details",
                        DisplayName = "Visualizar",
                        Icon = "fas fa-eye",
                        Url = "/Vendedores/Details/{id}"
                    },
                    new()
                    {
                        Name = "Edit",
                        DisplayName = "Editar",
                        Icon = "fas fa-edit",
                        Url = "/Vendedores/Edit/{id}"
                    },
                    new()
                    {
                        Name = "Sales",
                        DisplayName = "Vendas",
                        Icon = "fas fa-chart-line",
                        Url = "/Relatorios/VendasVendedor/{id}",
                        ShowCondition = (x) => ((Vendedor)x).Ativo == true
                    },
                    new()
                    {
                        Name = "ToggleStatus",
                        DisplayName = "Inativar",
                        Icon = "fas fa-ban",
                        Url = "/Vendedores/ToggleStatus/{id}",
                        ShowCondition = (x) => ((Vendedor)x).Ativo == true
                    },
                    new()
                    {
                        Name = "ToggleStatus",
                        DisplayName = "Ativar",
                        Icon = "fas fa-check",
                        Url = "/Vendedores/ToggleStatus/{id}",
                        ShowCondition = (x) => ((Vendedor)x).Ativo == false
                    }
                ]
            };
        }

        protected override IQueryable<Vendedor> ApplyFilters(IQueryable<Vendedor> query, Dictionary<string, object> filters)
        {
            // 3️⃣ Filtros (3 minutos para implementar)
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            // 🚀 Usar helper para múltiplas propriedades
                            query = ApplyTextFilter(query, searchTerm,
                                v => v.Nome,
                                v => v.CPF,
                                v => v.Email,
                                v => v.Telefone,
                                v => v.Celular);
                        }
                        break;

                    case "status":
                        if (bool.TryParse(filter.Value.ToString(), out bool status))
                        {
                            query = query.Where(v => v.Ativo == status);
                        }
                        break;

                    case "meta_min":
                        if (decimal.TryParse(filter.Value.ToString(), out decimal metaMin))
                        {
                            query = query.Where(v => v.Meta >= metaMin);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<Vendedor> ApplySort(IQueryable<Vendedor> query, string orderBy, string orderDirection)
        {
            // 4️⃣ Ordenação (2 minutos para implementar)
            return orderBy?.ToLower() switch
            {
                "id" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Id)
                    : query.OrderBy(v => v.Id),
                "nome" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Nome)
                    : query.OrderBy(v => v.Nome),
                "cpf" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.CPF)
                    : query.OrderBy(v => v.CPF),
                "email" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Email)
                    : query.OrderBy(v => v.Email),
                "percentualcomissao" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.PercentualComissao)
                    : query.OrderBy(v => v.PercentualComissao),
                "meta" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Meta)
                    : query.OrderBy(v => v.Meta),
                "ativo" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Ativo)
                    : query.OrderBy(v => v.Ativo),
                _ => query.OrderBy(v => v.Nome) // Default: ordem alfabética
            };
        }

        #endregion

        #region Ações Específicas (Opcionais - adicionar conforme necessário)

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.Ativo = !vendedor.Ativo;
                vendedor.DataAlteracao = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Vendedor {(vendedor.Ativo ? "ativado" : "inativado")} com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Vendedor não encontrado!";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vendedor = await _context.Vendedores
                .Include(v => v.Vendas)
                .Include(v => v.Avaliacoes)
                .Include(v => v.Tarefas)
                .FirstOrDefaultAsync(m => m.Id == id);

            return vendedor == null ? NotFound() : View(vendedor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vendedor vendedor)
        {
            if (ModelState.IsValid)
            {
                // Verificar CPF único
                var existeCPF = await _context.Vendedores.AnyAsync(v => v.CPF == vendedor.CPF);
                if (existeCPF)
                {
                    ModelState.AddModelError("CPF", "Já existe um vendedor com este CPF");
                    return View(vendedor);
                }

                vendedor.DataCadastro = DateTime.Now;
                vendedor.DataAlteracao = DateTime.Now;
                _context.Add(vendedor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vendedor cadastrado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(vendedor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vendedor = await _context.Vendedores.FindAsync(id);
            return vendedor == null ? NotFound() : View(vendedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vendedor vendedor)
        {
            if (id != vendedor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar CPF único (exceto o próprio)
                    var existeCPF = await _context.Vendedores.AnyAsync(v => v.CPF == vendedor.CPF && v.Id != vendedor.Id);
                    if (existeCPF)
                    {
                        ModelState.AddModelError("CPF", "Já existe outro vendedor com este CPF");
                        return View(vendedor);
                    }

                    vendedor.DataAlteracao = DateTime.Now;
                    _context.Update(vendedor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vendedor atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendedorExists(vendedor.Id))
                        return NotFound();
                    throw;
                }
            }
            return View(vendedor);
        }

        private bool VendedorExists(int id)
        {
            return _context.Vendedores.Any(e => e.Id == id);
        }

        #endregion
    }
}