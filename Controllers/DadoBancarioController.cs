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
    public class DadoBancarioController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<DadoBancario>> logger)
        : StandardGridController<DadoBancario>(context, fileStorageService, logger)
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
                    Placeholder = "Empresa, Banco, Agência, Conta..."
                },
                new()
                {
                    Name = "tipoconta",
                    DisplayName = "Tipo de Conta",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os tipos...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumTipoConta>(true)
                },
                new()
                {
                    Name = "principal",
                    DisplayName = "Conta Principal",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todas...",
                    Options =
                    [
                        new() { Value = "true", Text = "⭐ Principal" },
                        new() { Value = "false", Text = "📋 Secundária" }
                    ]
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<DadoBancario> ApplyFilters(IQueryable<DadoBancario> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = query.Where(d =>
                                (d.EmpresaCliente != null && d.EmpresaCliente.RazaoSocial.Contains(searchTerm)) ||
                                d.NomeBanco.Contains(searchTerm) ||
                                d.CodigoBanco.Contains(searchTerm) ||
                                d.Agencia.Contains(searchTerm) ||
                                d.NumeroConta.Contains(searchTerm));
                        }
                        break;

                    case "tipoconta":
                        query = ApplyEnumFilter(query, filters, filter.Key, d => d.TipoConta);
                        break;

                    case "principal":
                        if (bool.TryParse(filter.Value.ToString(), out bool principal))
                        {
                            query = query.Where(d => d.ContaPrincipal == principal);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<DadoBancario> GetBaseQuery()
        {
            return base.GetBaseQuery().Include(d => d.EmpresaCliente);
        }
    }
}
