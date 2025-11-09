using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class NotaFiscalController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<NotaFiscal>> logger)
        : StandardGridController<NotaFiscal>(context, fileStorageService, logger)
    {
        protected override StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel standardGridViewModel)
        {
            standardGridViewModel.Filters =
            [
                new()
                {
                    Name = "search",
                    DisplayName = "Busca",
                    Type = EnumGridFilterType.Text,
                    Placeholder = "Número, Chave de Acesso..."
                },
                new()
                {
                    Name = "status",
                    DisplayName = "Status",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os status...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumStatusNotaFiscal>(true)
                },
                new()
                {
                    Name = "tipo",
                    DisplayName = "Tipo",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os tipos...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumTipoNotaFiscal>(true)
                }
            ];

            standardGridViewModel.RowActions.AddRange(
            [
                new()
                {
                    Name = "DownloadXML",
                    DisplayName = "Download XML",
                    Icon = "fas fa-file-code",
                    Url = "/NotaFiscal/DownloadXML/{id}",
                    ShowCondition = (x) => !string.IsNullOrEmpty(((NotaFiscal)x).ArquivoXML)
                },
                new()
                {
                    Name = "DownloadPDF",
                    DisplayName = "Download PDF",
                    Icon = "fas fa-file-pdf",
                    Url = "/NotaFiscal/DownloadPDF/{id}",
                    ShowCondition = (x) => !string.IsNullOrEmpty(((NotaFiscal)x).ArquivoPDF)
                }
            ]);

            return standardGridViewModel;
        }

        protected override IQueryable<NotaFiscal> ApplyFilters(IQueryable<NotaFiscal> query, Dictionary<string, object> filters)
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
                                n => n.Numero.ToString(),
                                n => n.ChaveAcesso);
                        }
                        break;

                    case "status":
                        query = ApplyEnumFilter(query, filters, filter.Key, n => n.Status);
                        break;

                    case "tipo":
                        query = ApplyEnumFilter(query, filters, filter.Key, n => n.Tipo);
                        break;

                    case "modelo":
                        query = ApplyEnumFilter(query, filters, filter.Key, n => n.Modelo);
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<NotaFiscal> GetBaseQuery()
        {
            return base.GetBaseQuery().Include(n => n.EmpresaCliente);
        }
    }
}
