using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models.Grid;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class EmpresaController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Empresa>> logger)
        : StandardGridController<Empresa>(context, fileStorageService, logger)
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
                    Placeholder = "Razão Social, CNPJ, Cidade..."
                },
                new()
                {
                    Name = "estado",
                    DisplayName = "Estado",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os estados...",
                    Options = EnumExtension.GetSelectListItems<EnumEstado>(true)
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<Empresa> ApplyFilters(IQueryable<Empresa> query, Dictionary<string, object> filters)
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
                                e => e.Cnpj,
                                e => e.Cidade);
                        }
                        break;

                    case "estado":
                        query = ApplyEnumFilter(query, filters, filter.Key, e => e.Estado);
                        break;
                }
            }

            return query;
        }
    }
}