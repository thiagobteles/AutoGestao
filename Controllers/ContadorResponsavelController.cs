using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Controllers
{
    public class ContadorResponsavelController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<ContadorResponsavel>> logger)
        : StandardGridController<ContadorResponsavel>(context, fileStorageService, logger)
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
                    Placeholder = "Nome, CPF, CRC, Email..."
                },
                new()
                {
                    Name = "estadocrc",
                    DisplayName = "Estado do CRC",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os estados...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.EnumEstado>(true)
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

        protected override IQueryable<ContadorResponsavel> ApplyFilters(IQueryable<ContadorResponsavel> query, Dictionary<string, object> filters)
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
                                c => c.CRC,
                                c => c.Email,
                                c => c.Escritorio);
                        }
                        break;

                    case "estadocrc":
                        query = ApplyEnumFilter(query, filters, filter.Key, c => c.EstadoCRC);
                        break;

                    case "status":
                        if (bool.TryParse(filter.Value.ToString(), out bool status))
                        {
                            query = query.Where(c => c.Ativo == status);
                        }
                        break;
                }
            }

            return query;
        }
    }
}
