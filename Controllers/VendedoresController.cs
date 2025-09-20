using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Extensions;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class VendedoresController(ApplicationDbContext context) : StandardGridController<Vendedor>(context)
    {
        #region Implementação Obrigatória

        protected override IQueryable<Vendedor> GetBaseQuery()
        {
            return _context.Vendedores.AsQueryable();
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            var retorno = new StandardGridViewModel ("Vendedores", "Gerencie todos os vendedores", "Vendedores")
            {
                Columns =
                [
                    new() { Name = nameof(Vendedor.Id), DisplayName = "Cód", Type = GridColumnType.Number, Sortable = true, Width = "65px" },
                    new() { Name = nameof(Vendedor.Nome), DisplayName = "Nome Completo", Sortable = true },
                    new() { Name = nameof(Vendedor.CPF), DisplayName = "CPF", Sortable = true, Type = GridColumnType.Custom, CustomRender = RenderDocumento },
                    new() { Name = nameof(Vendedor.Email), DisplayName = "Email", Sortable = true },
                    new() { Name = nameof(Vendedor.Celular), DisplayName = "Celular", Sortable = true },
                    new() { Name = nameof(Vendedor.Ativo), DisplayName = "Ativo", Type = GridColumnType.Enumerador, Sortable = true, Width = "65px" },
                    new() { Name = "Actions", DisplayName = "Ações", Type = GridColumnType.Actions, Sortable = false, Width = "100px" }
                ],

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
                    }
                ],
            };

            retorno.RowActions.AddRange(
                [
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
                ]);

            return retorno;
        }

        protected override IQueryable<Vendedor> ApplyFilters(IQueryable<Vendedor> query, Dictionary<string, object> filters)
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

        private static string RenderDocumento(object item)
        {
            var vendedor = (Vendedor)item;
            return vendedor.CPF.AplicarMascaraCpf();
        }

        #region Ações Específicas

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
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedores
                .Include(v => v.Vendas)
                .Include(v => v.Avaliacoes)
                .Include(v => v.Tarefas)
                .FirstOrDefaultAsync(m => m.Id == id);

            return vendedor == null ? NotFound() : View(vendedor);
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vendedor vendedor)
        {
            if (id != vendedor.Id)
            {
                return NotFound();
            }

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
                    {
                        return NotFound();
                    }

                    throw;
                }
            }
            return View(vendedor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedores.FindAsync(id);
            return vendedor == null ? NotFound() : View(vendedor);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Vendedor vendedor)
        {
            if (id != vendedor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Remove(vendedor);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vendedor deletado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendedorExists(vendedor.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(vendedor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor == null)
            {
                return NotFound();
            }

            _context.Remove(vendedor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vendedor deletado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            try
            {
                var vendedores = await _context.Vendedores
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("ID,Nome,CPF,Email,Telefone,Celular,Status,Data Cadastro");

                foreach (var vendedor in vendedores)
                {
                    csv.AppendLine($"{vendedor.Id}," +
                                  $"\"{vendedor.Nome}\"," +
                                  $"{vendedor.CPF}," +
                                  $"{vendedor.Email}," +
                                  $"{vendedor.Telefone}," +
                                  $"{vendedor.Celular}," +
                                  $"{(vendedor.Ativo ? "Ativo" : "Inativo")}," +
                                  $"{vendedor.DataCadastro:dd/MM/yyyy}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"vendedores_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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
            return RedirectToAction();
        }

        private bool VendedorExists(int id)
        {
            return _context.Vendedores.Any(e => e.Id == id);
        }

        #endregion
    }
}