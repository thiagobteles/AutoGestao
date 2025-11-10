using FGT.Controllers.Base;
using FGT.Data;
using FGT.Entidades;
using FGT.Enumerador.Gerais;
using FGT.Extensions;
using FGT.Models;
using FGT.Models.Grid;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FGT.Controllers
{
    public class EmpresaClienteController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<EmpresaCliente>> logger)
        : StandardGridController<EmpresaCliente>(context, fileStorageService, logger)
    {
        protected override StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel standardGridViewModel)
        {
            standardGridViewModel.Filters =
            [
                new()
                {
                    Name = "search",
                    DisplayName = "Busca Geral",
                    Type = EnumGridFilterType.Text,
                    Placeholder = "Razão Social, CNPJ, Nome Fantasia..."
                },
                new()
                {
                    Name = "regimetributario",
                    DisplayName = "Regime Tributário",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os regimes...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumRegimeTributario>(true)
                },
                new()
                {
                    Name = "status",
                    DisplayName = "Status",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Status...",
                    Options =
                    [
                        new() { Value = "true", Text = "✅ Ativo" },
                        new() { Value = "false", Text = "❌ Inativo" }
                    ]
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<EmpresaCliente> ApplyFilters(IQueryable<EmpresaCliente> query, Dictionary<string, object> filters)
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
                                e => e.RazaoSocial,
                                e => e.NomeFantasia,
                                e => e.CNPJ);
                        }
                        break;

                    case "status":
                        if (bool.TryParse(filter.Value.ToString(), out bool status))
                        {
                            query = query.Where(e => e.Ativo == status);
                        }
                        break;

                    case "regimetributario":
                        query = ApplyEnumFilter(query, filters, filter.Key, e => e.RegimeTributario);
                        break;
                }
            }

            return query;
        }
    }
}
