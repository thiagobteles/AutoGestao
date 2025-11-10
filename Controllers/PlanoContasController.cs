using FGT.Controllers.Base;
using FGT.Data;
using FGT.Entidades;
using FGT.Enumerador.Gerais;
using FGT.Extensions;
using FGT.Models;
using FGT.Models.Grid;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FGT.Controllers
{
    public class PlanoContasController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<PlanoContas>> logger)
        : StandardGridController<PlanoContas>(context, fileStorageService, logger)
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
                    Placeholder = "Código, Descrição..."
                },
                new()
                {
                    Name = "natureza",
                    DisplayName = "Natureza",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todas...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumNaturezaConta>(true)
                },
                new()
                {
                    Name = "analitica",
                    DisplayName = "Tipo de Conta",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todas...",
                    Options =
                    [
                        new() { Value = "true", Text = "📊 Analítica" },
                        new() { Value = "false", Text = "📁 Sintética" }
                    ]
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<PlanoContas> ApplyFilters(IQueryable<PlanoContas> query, Dictionary<string, object> filters)
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
                                p => p.Codigo,
                                p => p.Descricao);
                        }
                        break;

                    case "tipoconta":
                        query = ApplyEnumFilter(query, filters, filter.Key, p => p.TipoConta);
                        break;

                    case "natureza":
                        query = ApplyEnumFilter(query, filters, filter.Key, p => p.Natureza);
                        break;

                    case "analitica":
                        if (bool.TryParse(filter.Value.ToString(), out bool analitica))
                        {
                            query = query.Where(p => p.ContaAnalitica == analitica);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<PlanoContas> GetBaseQuery()
        {
            return base.GetBaseQuery().Include(p => p.ContaPai);
        }
    }
}
