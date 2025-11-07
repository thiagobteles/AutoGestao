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
    public class LancamentoContabilController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<LancamentoContabil>> logger)
        : StandardGridController<LancamentoContabil>(context, fileStorageService, logger)
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
                    Placeholder = "Histórico, Documento..."
                },
                new()
                {
                    Name = "tipolancamento",
                    DisplayName = "Tipo de Lançamento",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os tipos...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumTipoLancamento>(true)
                },
                new()
                {
                    Name = "conciliado",
                    DisplayName = "Conciliação",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todas...",
                    Options =
                    [
                        new() { Value = "true", Text = "✅ Conciliado" },
                        new() { Value = "false", Text = "⏳ Pendente" }
                    ]
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<LancamentoContabil> ApplyFilters(IQueryable<LancamentoContabil> query, Dictionary<string, object> filters)
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
                                l => l.Historico,
                                l => l.NumeroDocumento,
                                l => l.Complemento);
                        }
                        break;

                    case "tipolancamento":
                        query = ApplyEnumFilter(query, filters, filter.Key, l => l.TipoLancamento);
                        break;

                    case "conciliado":
                        if (bool.TryParse(filter.Value.ToString(), out bool conciliado))
                        {
                            query = query.Where(l => l.Conciliado == conciliado);
                        }
                        break;

                    case "datainicio":
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dataInicio))
                        {
                            query = query.Where(l => l.DataLancamento >= dataInicio);
                        }
                        break;

                    case "datafim":
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dataFim))
                        {
                            query = query.Where(l => l.DataLancamento <= dataFim);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<LancamentoContabil> GetBaseQuery()
        {
            return base.GetBaseQuery()
                .Include(l => l.EmpresaCliente)
                .Include(l => l.ContaDebito)
                .Include(l => l.ContaCredito)
                .Include(l => l.NotaFiscal);
        }
    }
}
