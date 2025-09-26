using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class VendedoresController(ApplicationDbContext context) : StandardGridController<Vendedor>(context)
    {
        protected override StandardGridViewModel ConfigureGrid()
        {
            var retorno = new StandardGridViewModel ("Vendedores", "Gerencie todos os vendedores", "Vendedores")
            {
                Columns =
                [
                    new() { Name = nameof(Vendedor.Id), DisplayName = "Cód", Type = EnumGridColumnType.Number, Sortable = true, Width = "65px" },
                    new() { Name = nameof(Vendedor.Nome), DisplayName = "Nome Completo", Sortable = true },
                    new() { Name = nameof(Vendedor.CPF), DisplayName = "CPF", Sortable = true, Type = EnumGridColumnType.Custom, CustomRender = RenderDocumento },
                    new() { Name = nameof(Vendedor.Email), DisplayName = "Email", Sortable = true },
                    new() { Name = nameof(Vendedor.Celular), DisplayName = "Celular", Sortable = true },
                    new() { Name = nameof(Vendedor.Ativo), DisplayName = "Ativo", Type = EnumGridColumnType.Enumerador, Sortable = true, Width = "65px" },
                    new() { Name = "Actions", DisplayName = "Ações", Type = EnumGridColumnType.Actions, Sortable = false, Width = "100px" }
                ],

                Filters =
                [
                    new()
                    {
                        Name = "search",
                        DisplayName = "Busca Geral",
                        Type = EnumGridFilterType.Text,
                        Placeholder = "Nome, CPF, Email..."
                    },
                    new()
                    {
                        Name = "status",
                        DisplayName = "Status",
                        Type = EnumGridFilterType.Select,
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
                        Name = "AlterarStatus",
                        DisplayName = "Inativar",
                        Icon = "fas fa-ban",
                        Url = "/Vendedores/AlterarStatus/{id}",
                        ShowCondition = (x) => ((Vendedor)x).Ativo == true
                    },
                    new()
                    {
                        Name = "AlterarStatus",
                        DisplayName = "Ativar",
                        Icon = "fas fa-check",
                        Url = "/Vendedores/AlterarStatus/{id}",
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
                }
            }

            return query;
        }

        private static string RenderDocumento(object item)
        {
            var vendedor = (Vendedor)item;
            return vendedor.CPF.AplicarMascaraCpf();
        }

        #region Ações Específicas

        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int id)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.Ativo = !vendedor.Ativo;
                vendedor.DataAlteracao = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Vendedor {(vendedor.Ativo ? "ativado" : "inativado")} com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Vendedor não encontrado!";
            }

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
                var fileName = $"vendedores_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao exportar dados: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion
    }
}