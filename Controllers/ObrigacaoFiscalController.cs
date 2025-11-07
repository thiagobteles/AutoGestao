using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class ObrigacaoFiscalController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<ObrigacaoFiscal>> logger)
        : StandardGridController<ObrigacaoFiscal>(context, fileStorageService, logger)
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
                    Placeholder = "Empresa, Recibo..."
                },
                new()
                {
                    Name = "tipoobrigacao",
                    DisplayName = "Tipo de Obrigação",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os tipos...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumTipoObrigacaoFiscal>(true)
                },
                new()
                {
                    Name = "status",
                    DisplayName = "Status",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os status...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumStatusObrigacao>(true)
                },
                new()
                {
                    Name = "periodicidade",
                    DisplayName = "Periodicidade",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todas...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumPeriodicidade>(true)
                },
                new()
                {
                    Name = "competencia",
                    DisplayName = "Competência",
                    Type = EnumGridFilterType.Date
                }
            ];

            standardGridViewModel.RowActions.AddRange(
            [
                new()
                {
                    Name = "DownloadArquivo",
                    DisplayName = "Download Arquivo",
                    Icon = "fas fa-download",
                    Url = "/ObrigacaoFiscal/DownloadArquivo/{id}",
                    ShowCondition = (x) => !string.IsNullOrEmpty(((ObrigacaoFiscal)x).ArquivoEnviado)
                },
                new()
                {
                    Name = "DownloadRecibo",
                    DisplayName = "Download Recibo",
                    Icon = "fas fa-file-pdf",
                    Url = "/ObrigacaoFiscal/DownloadRecibo/{id}",
                    ShowCondition = (x) => !string.IsNullOrEmpty(((ObrigacaoFiscal)x).ArquivoRecibo)
                }
            ]);

            return standardGridViewModel;
        }

        protected override IQueryable<ObrigacaoFiscal> ApplyFilters(IQueryable<ObrigacaoFiscal> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = query.Where(o =>
                                (o.EmpresaCliente != null && o.EmpresaCliente.RazaoSocial.Contains(searchTerm)) ||
                                (o.NumeroRecibo != null && o.NumeroRecibo.Contains(searchTerm)));
                        }
                        break;

                    case "tipoobrigacao":
                        query = ApplyEnumFilter(query, filters, filter.Key, o => o.TipoObrigacao);
                        break;

                    case "status":
                        query = ApplyEnumFilter(query, filters, filter.Key, o => o.Status);
                        break;

                    case "periodicidade":
                        query = ApplyEnumFilter(query, filters, filter.Key, o => o.Periodicidade);
                        break;

                    case "competencia":
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime competencia))
                        {
                            query = query.Where(o => o.Competencia.Year == competencia.Year &&
                                                     o.Competencia.Month == competencia.Month);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<ObrigacaoFiscal> GetBaseQuery()
        {
            return base.GetBaseQuery().Include(o => o.EmpresaCliente);
        }
    }
}
