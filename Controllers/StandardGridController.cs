using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public abstract class StandardGridController<T> : BaseController where T : BaseEntidade
    {
        protected readonly ApplicationDbContext _context;

        protected StandardGridController(ApplicationDbContext context) : base()
        {
            _context = context;
        }

        protected abstract IQueryable<T> GetBaseQuery();
        protected abstract StandardGridViewModel ConfigureGrid();
        protected abstract IQueryable<T> ApplyFilters(IQueryable<T> query, Dictionary<string, object> filters);
        protected abstract IQueryable<T> ApplySort(IQueryable<T> query, string orderBy, string orderDirection);

        [HttpGet]
        public virtual async Task<IActionResult> Index(
            string? search = null,
            string? orderBy = null,
            string? orderDirection = "asc",
            int pageSize = 50,
            int page = 1)
        {
            var gridConfig = ConfigureGrid();
            var query = GetBaseQuery();

            // Aplicar filtros
            var filters = ExtractFiltersFromRequest();
            if (!string.IsNullOrEmpty(search))
                filters["search"] = search;

            query = ApplyFilters(query, filters);

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(orderBy))
                query = ApplySort(query, orderBy, orderDirection);

            // Obter total de registros
            var totalRecords = await query.CountAsync();

            // Aplicar paginação
            List<T> items;
            if (pageSize == -1)
            {
                items = await query.ToListAsync();
            }
            else
            {
                items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            // Configurar ViewModel - CONVERSÃO DIRETA
            gridConfig.Items = items.Cast<object>().ToList(); // CONVERSÃO EXPLÍCITA
            gridConfig.TotalRecords = totalRecords;
            gridConfig.CurrentPage = page;
            gridConfig.PageSize = pageSize;
            gridConfig.Search = search;
            gridConfig.OrderBy = orderBy;
            gridConfig.OrderDirection = orderDirection;

            // Atualizar valores dos filtros
            UpdateFilterValues(gridConfig.Filters, filters);

            return View("_StandardGrid", gridConfig);
        }

        private Dictionary<string, object> ExtractFiltersFromRequest()
        {
            var filters = new Dictionary<string, object>();

            foreach (var key in Request.Query.Keys)
            {
                if (!string.IsNullOrEmpty(Request.Query[key]) &&
                    !new[] { "page", "pageSize", "orderBy", "orderDirection" }.Contains(key))
                {
                    filters[key] = Request.Query[key].ToString();
                }
            }

            return filters;
        }

        private void UpdateFilterValues(List<GridFilter> filters, Dictionary<string, object> values)
        {
            foreach (var filter in filters)
            {
                if (values.ContainsKey(filter.Name))
                {
                    filter.Value = values[filter.Name];
                }
            }
        }
    }
}