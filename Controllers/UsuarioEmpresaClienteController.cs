using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoGestao.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuarioEmpresaClienteController(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<StandardGridController<UsuarioEmpresaCliente>> logger)
        : StandardGridController<UsuarioEmpresaCliente>(context, fileStorageService, logger)
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
                    Placeholder = "Empresa Cliente..."
                },
                new()
                {
                    Name = "ativo",
                    DisplayName = "Status",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os status...",
                    Options =
                    [
                        new SelectListItem { Value = "", Text = "Todos" },
                        new SelectListItem { Value = "true", Text = "✅ Ativos" },
                        new SelectListItem { Value = "false", Text = "⛔ Inativos" }
                    ]
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<UsuarioEmpresaCliente> ApplyFilters(
            IQueryable<UsuarioEmpresaCliente> query,
            Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = query.Where(ue =>
                                ue.EmpresaCliente != null && (
                                    ue.EmpresaCliente.RazaoSocial.Contains(searchTerm) ||
                                    ue.EmpresaCliente.NomeFantasia != null && ue.EmpresaCliente.NomeFantasia.Contains(searchTerm) ||
                                    ue.EmpresaCliente.CNPJ != null && ue.EmpresaCliente.CNPJ.Contains(searchTerm)
                                )
                            );
                        }
                        break;

                    case "ativo":
                        var ativoFilter = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(ativoFilter) && bool.TryParse(ativoFilter, out bool ativo))
                        {
                            query = query.Where(ue => ue.Ativo == ativo);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<UsuarioEmpresaCliente> GetBaseQuery()
        {
            // Sempre incluir as navigation properties para exibição na grid
            return base.GetBaseQuery();
        }
    }
}
