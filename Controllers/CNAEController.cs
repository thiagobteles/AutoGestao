using FGT.Controllers.Base;
using FGT.Data;
using FGT.Entidades;
using FGT.Enumerador.Gerais;
using FGT.Models;
using FGT.Models.Grid;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FGT.Controllers
{
    public class CNAEController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<CNAE>> logger)
        : StandardGridController<CNAE>(context, fileStorageService, logger)
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
                    Placeholder = "Código ou Descrição..."
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<CNAE> ApplyFilters(IQueryable<CNAE> query, Dictionary<string, object> filters)
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
                                c => c.Codigo,
                                c => c.Descricao);
                        }
                        break;
                }
            }

            return query;
        }
    }
}
