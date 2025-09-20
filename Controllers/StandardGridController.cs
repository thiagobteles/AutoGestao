using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Extensions;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public abstract class StandardGridController<T>(ApplicationDbContext context) : Controller() where T : BaseEntidade
    {
        protected readonly ApplicationDbContext _context = context;
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
            {
                filters["search"] = search;
            }

            query = ApplyFilters(query, filters);

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplySort(query, orderBy, orderDirection);
            }

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

            // Configurar ViewModel
            gridConfig.Items = items.Cast<object>().ToList();
            gridConfig.TotalRecords = totalRecords;
            gridConfig.CurrentPage = page;
            gridConfig.PageSize = pageSize;
            gridConfig.Search = search;
            gridConfig.OrderBy = orderBy;
            gridConfig.OrderDirection = orderDirection;

            // Atualizar valores dos filtros
            UpdateFilterValues(gridConfig.Filters, filters);

            return View("_StandardGridContent", gridConfig);
        }

        /// <summary>
        /// Endpoint AJAX para atualização dinâmica da grid
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> GetDataAjax(
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
            {
                filters["search"] = search;
            }

            query = ApplyFilters(query, filters);

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplySort(query, orderBy, orderDirection);
            }

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

            // Configurar ViewModel
            gridConfig.Items = items.Cast<object>().ToList();
            gridConfig.TotalRecords = totalRecords;
            gridConfig.CurrentPage = page;
            gridConfig.PageSize = pageSize;
            gridConfig.Search = search;
            gridConfig.OrderBy = orderBy;
            gridConfig.OrderDirection = orderDirection;

            // Atualizar valores dos filtros
            UpdateFilterValues(gridConfig.Filters, filters);

            return PartialView("_StandardGridContent", gridConfig);
        }

        private Dictionary<string, object> ExtractFiltersFromRequest()
        {
            var filters = new Dictionary<string, object>();

            foreach (var key in Request.Query.Keys)
            {
                var value = Request.Query[key].ToString();

                if (!string.IsNullOrEmpty(value) &&
                    !new[] { "page", "pageSize", "orderBy", "orderDirection" }.Contains(key))
                {
                    // Tratamento especial para filtros de data range
                    if (key.EndsWith("_inicio") || key.EndsWith("_fim"))
                    {
                        filters[key] = value;
                    }
                    else
                    {
                        filters[key] = value;
                    }
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

                // Para filtros de data range, verificar campos específicos
                if (filter.Type == GridFilterType.DateRange)
                {
                    var inicioKey = $"{filter.Name}_inicio";
                    var fimKey = $"{filter.Name}_fim";

                    if (values.ContainsKey(inicioKey))
                    {
                        ViewBag.GetType().GetProperty(inicioKey)?.SetValue(ViewBag, values[inicioKey]);
                    }

                    if (values.ContainsKey(fimKey))
                    {
                        ViewBag.GetType().GetProperty(fimKey)?.SetValue(ViewBag, values[fimKey]);
                    }
                }
            }
        }

        /// <summary>
        /// Helper para aplicar filtros de data range
        /// </summary>
        protected IQueryable<T> ApplyDateRangeFilter<TProperty>(
            IQueryable<T> query,
            Dictionary<string, object> filters,
            string filterName,
            System.Linq.Expressions.Expression<Func<T, TProperty>> propertyExpression)
        {
            var inicioKey = $"{filterName}_inicio";
            var fimKey = $"{filterName}_fim";

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (filters.ContainsKey(inicioKey) && DateTime.TryParse(filters[inicioKey].ToString(), out var inicio))
            {
                dataInicio = inicio;
            }

            if (filters.ContainsKey(fimKey) && DateTime.TryParse(filters[fimKey].ToString(), out var fim))
            {
                dataFim = fim.Date.AddDays(1).AddTicks(-1); // Fim do dia
            }

            if (dataInicio.HasValue || dataFim.HasValue)
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                var property = System.Linq.Expressions.Expression.Property(parameter, ((System.Linq.Expressions.MemberExpression)propertyExpression.Body).Member.Name);

                System.Linq.Expressions.Expression? condition = null;

                if (dataInicio.HasValue)
                {
                    var inicioConstant = System.Linq.Expressions.Expression.Constant(dataInicio.Value);
                    var greaterThanOrEqual = System.Linq.Expressions.Expression.GreaterThanOrEqual(property, inicioConstant);
                    condition = condition == null ? greaterThanOrEqual : System.Linq.Expressions.Expression.AndAlso(condition, greaterThanOrEqual);
                }

                if (dataFim.HasValue)
                {
                    var fimConstant = System.Linq.Expressions.Expression.Constant(dataFim.Value);
                    var lessThanOrEqual = System.Linq.Expressions.Expression.LessThanOrEqual(property, fimConstant);
                    condition = condition == null ? lessThanOrEqual : System.Linq.Expressions.Expression.AndAlso(condition, lessThanOrEqual);
                }

                if (condition != null)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(condition, parameter);
                    query = query.Where(lambda);
                }
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros de texto com múltiplas propriedades
        /// </summary>
        protected IQueryable<T> ApplyTextFilter(
            IQueryable<T> query,
            string searchTerm,
            params System.Linq.Expressions.Expression<Func<T, string?>>[] properties)
        {
            if (string.IsNullOrEmpty(searchTerm) || properties.Length == 0)
            {
                return query;
            }

            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            System.Linq.Expressions.Expression? condition = null;

            foreach (var propertyExpr in properties)
            {
                var propertyName = ((System.Linq.Expressions.MemberExpression)propertyExpr.Body).Member.Name;
                var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);

                // Verificar se não é null
                var notNull = System.Linq.Expressions.Expression.NotEqual(property, System.Linq.Expressions.Expression.Constant(null));

                // Aplicar Contains
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchConstant = System.Linq.Expressions.Expression.Constant(searchTerm);
                var containsCall = System.Linq.Expressions.Expression.Call(property, containsMethod!, searchConstant);

                // Combinar null check com contains
                var propertyCondition = System.Linq.Expressions.Expression.AndAlso(notNull, containsCall);

                condition = condition == null ? propertyCondition : System.Linq.Expressions.Expression.OrElse(condition, propertyCondition);
            }

            if (condition != null)
            {
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(condition, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros de enum
        /// </summary>
        protected IQueryable<T> ApplyEnumFilter<TEnum>(
            IQueryable<T> query,
            Dictionary<string, object> filters,
            string filterName,
            System.Linq.Expressions.Expression<Func<T, TEnum>> propertyExpression) where TEnum : struct, Enum
        {
            if (filters.ContainsKey(filterName) &&
                Enum.TryParse<TEnum>(filters[filterName].ToString(), out TEnum enumValue))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                var property = System.Linq.Expressions.Expression.Property(parameter, ((System.Linq.Expressions.MemberExpression)propertyExpression.Body).Member.Name);
                var constant = System.Linq.Expressions.Expression.Constant(enumValue);
                var equal = System.Linq.Expressions.Expression.Equal(property, constant);
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equal, parameter);

                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros numéricos
        /// </summary>
        protected IQueryable<T> ApplyNumericFilter<TProperty>(
            IQueryable<T> query,
            Dictionary<string, object> filters,
            string filterName,
            System.Linq.Expressions.Expression<Func<T, TProperty>> propertyExpression) where TProperty : struct, IComparable<TProperty>
        {
            if (filters.ContainsKey(filterName))
            {
                var value = filters[filterName].ToString();
                if (TryConvertToNumeric(value, out TProperty numericValue))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                    var property = System.Linq.Expressions.Expression.Property(parameter, ((System.Linq.Expressions.MemberExpression)propertyExpression.Body).Member.Name);
                    var constant = System.Linq.Expressions.Expression.Constant(numericValue);
                    var equal = System.Linq.Expressions.Expression.Equal(property, constant);
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equal, parameter);

                    query = query.Where(lambda);
                }
            }

            return query;
        }

        private static bool TryConvertToNumeric<TProperty>(string value, out TProperty result)
        {
            result = default(TProperty);

            try
            {
                result = (TProperty)Convert.ChangeType(value, typeof(TProperty));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}