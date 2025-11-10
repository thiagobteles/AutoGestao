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
    public class ParametroFiscalController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<ParametroFiscal>> logger)
        : StandardGridController<ParametroFiscal>(context, fileStorageService, logger)
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
                    Placeholder = "Buscar empresa..."
                },
                new()
                {
                    Name = "ambiente",
                    DisplayName = "Ambiente NFe",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os ambientes...",
                    Options = EnumExtension.GetSelectListItems<Enumerador.Fiscal.EnumAmbienteNFe>(true)
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<ParametroFiscal> ApplyFilters(IQueryable<ParametroFiscal> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = query.Where(p => p.EmpresaCliente != null &&
                                (p.EmpresaCliente.RazaoSocial.Contains(searchTerm) ||
                                 p.EmpresaCliente.NomeFantasia!.Contains(searchTerm) ||
                                 p.EmpresaCliente.CNPJ.Contains(searchTerm)));
                        }
                        break;

                    case "ambiente":
                        query = ApplyEnumFilter(query, filters, filter.Key, p => p.AmbienteNFe);
                        break;
                }
            }

            return query;
        }

        protected override IQueryable<ParametroFiscal> GetBaseQuery()
        {
            return base.GetBaseQuery().Include(p => p.EmpresaCliente);
        }
    }
}
