using FGT.Controllers.Base;
using FGT.Data;
using FGT.Entidades;
using FGT.Enumerador.Gerais;
using FGT.Models;
using FGT.Models.Grid;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FGT.Controllers
{
    public class CertificadoDigitalController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<CertificadoDigital>> logger)
        : StandardGridController<CertificadoDigital>(context, fileStorageService, logger)
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
                    Placeholder = "Titular ou Empresa..."
                },
                new()
                {
                    Name = "validade",
                    DisplayName = "Status de Validade",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos...",
                    Options =
                    [
                        new SelectListItem { Value = "valido", Text = "✅ Válidos" },
                        new SelectListItem { Value = "expirado", Text = "⚠️ Expirados" },
                        new SelectListItem { Value = "proximovencimento", Text = "🔔 Vencimento próximo (30 dias)" }
                    ]
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<CertificadoDigital> ApplyFilters(IQueryable<CertificadoDigital> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = query.Where(c =>
                                c.Titular.Contains(searchTerm) ||
                                (c.EmpresaCliente != null && c.EmpresaCliente.RazaoSocial.Contains(searchTerm)));
                        }
                        break;

                    case "validade":
                        var validadeFilter = filter.Value.ToString();
                        var hoje = DateTime.Now.Date;
                        var daquiA30Dias = hoje.AddDays(30);

                        if (validadeFilter == "valido")
                        {
                            query = query.Where(c => c.DataValidade > hoje);
                        }
                        else if (validadeFilter == "expirado")
                        {
                            query = query.Where(c => c.DataValidade <= hoje);
                        }
                        else if (validadeFilter == "proximovencimento")
                        {
                            query = query.Where(c => c.DataValidade > hoje && c.DataValidade <= daquiA30Dias);
                        }
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<CertificadoDigital> GetBaseQuery()
        {
            return base.GetBaseQuery().Include(c => c.EmpresaCliente);
        }
    }
}
