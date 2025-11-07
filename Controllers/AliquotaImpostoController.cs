using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Controllers
{
    public class AliquotaImpostoController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<AliquotaImposto>> logger)
        : StandardGridController<AliquotaImposto>(context, fileStorageService, logger)
    {
        protected override StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel standardGridViewModel)
        {
            standardGridViewModel.Filters =
            [
                new()
                {
                    Name = "tipoimposto",
                    DisplayName = "Tipo de Imposto",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os impostos...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumTipoImposto>(true)
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
                    Name = "estado",
                    DisplayName = "Estado",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os estados...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.EnumEstado>(true)
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<AliquotaImposto> ApplyFilters(IQueryable<AliquotaImposto> query, Dictionary<string, object> filters)
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
                                e => e.Observacoes);
                        }
                        break;

                    case "tipoimposto":
                        query = ApplyEnumFilter(query, filters, filter.Key, e => e.TipoImposto);
                        break;

                    case "regimetributario":
                        if (Enum.TryParse<Enumerador.Fiscal.EnumRegimeTributario>(filter.Value.ToString(), out var regimeTributario))
                        {
                            query = query.Where(e => e.RegimeTributario == regimeTributario);
                        }
                        break;

                    case "estado":
                        if (Enum.TryParse<Enumerador.EnumEstado>(filter.Value.ToString(), out var estado))
                        {
                            query = query.Where(e => e.Estado == estado);
                        }
                        break;
                }
            }

            return query;
        }
    }
}
