using AutoGestao.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoGestao.Extensions
{
    /// <summary>
    /// Extensões para aplicação automática de ordenação e filtros em queries
    /// </summary>
    public static class QueryableAutoExtensions
    {
        /// <summary>
        /// Aplica ordenação automática baseada em metadados
        /// </summary>
        public static IQueryable<T> ApplyAutoOrdering<T>(
            this IQueryable<T> query, 
            string? orderBy, 
            string orderDirection,
            EntityMetadataCache<T> metadata) where T : class
        {
            var field = orderBy ?? metadata.DefaultOrderField;
            var property = metadata.GetPropertyByName(field);
            
            if (property == null)
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = orderDirection.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(T), property.PropertyType);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }

        /// <summary>
        /// Aplica filtros automáticos baseado nos valores
        /// </summary>
        public static IQueryable<T> ApplyAutoFilters<T>(
            this IQueryable<T> query,
            Dictionary<string, object> filters,
            EntityMetadataCache<T> metadata) where T : class
        {
            foreach (var filter in filters)
            {
                if (string.IsNullOrEmpty(filter.Value?.ToString()))
                {
                    continue;
                }

                var property = metadata.GetPropertyByName(filter.Key);
                if (property == null)
                {
                    continue;
                }

                query = ApplyPropertyFilter(query, property, filter.Value);
            }

            return query;
        }

        /// <summary>
        /// Aplica busca automática nos campos searchable
        /// </summary>
        public static IQueryable<T> ApplyAutoSearch<T>(
            this IQueryable<T> query,
            string searchTerm,
            EntityMetadataCache<T> metadata) where T : class
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }

            var searchableProperties = metadata.SearchableProperties;
            if (!searchableProperties.Any())
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var property in searchableProperties)
            {
                var propertyExpression = BuildSearchExpression(parameter, property, searchTerm);
                if (propertyExpression != null)
                {
                    combinedExpression = combinedExpression == null 
                        ? propertyExpression 
                        : Expression.OrElse(combinedExpression, propertyExpression);
                }
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Aplica includes automáticos para navigation properties
        /// </summary>
        public static IQueryable<T> ApplyAutoIncludes<T>(
            this IQueryable<T> query,
            EntityMetadataCache<T> metadata) where T : class
        {
            var navigationProperties = metadata.NavigationProperties;
            
            foreach (var navProp in navigationProperties)
            {
                // Incluir apenas se for usado na grid ou form
                if (IsPropertyUsedInDisplay(navProp, metadata))
                {
                    query = query.Include(navProp.Name);
                }
            }

            return query;
        }

        #region Private Methods

        private static IQueryable<T> ApplyPropertyFilter<T>(
            IQueryable<T> query, 
            PropertyInfo property, 
            object value) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            
            Expression? filterExpression = null;
            var valueString = value.ToString()!;

            // Diferentes tipos de filtro baseado no tipo da propriedade
            if (property.PropertyType == typeof(string))
            {
                filterExpression = BuildStringFilter(propertyAccess, valueString);
            }
            else if (IsNumericType(property.PropertyType))
            {
                filterExpression = BuildNumericFilter(propertyAccess, valueString, property.PropertyType);
            }
            else if (IsEnumType(property.PropertyType))
            {
                filterExpression = BuildEnumFilter(propertyAccess, valueString, property.PropertyType);
            }
            else if (IsDateType(property.PropertyType))
            {
                filterExpression = BuildDateFilter(propertyAccess, valueString, property.PropertyType);
            }
            else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {
                filterExpression = BuildBooleanFilter(propertyAccess, valueString);
            }

            if (filterExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        private static Expression? BuildSearchExpression(
            ParameterExpression parameter, 
            PropertyInfo property, 
            string searchTerm)
        {
            var propertyAccess = Expression.Property(parameter, property);

            if (property.PropertyType == typeof(string))
            {
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                var searchValue = Expression.Constant(searchTerm, typeof(string));
                
                // Verificar se a propriedade não é null antes de chamar Contains
                var notNullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                var containsCall = Expression.Call(propertyAccess, containsMethod, searchValue);
                
                return Expression.AndAlso(notNullCheck, containsCall);
            }
            else if (IsNumericType(property.PropertyType))
            {
                // Para campos numéricos, verificar se o termo de busca é um número válido
                if (TryParseNumeric(searchTerm, property.PropertyType, out var numericValue))
                {
                    var equalExpression = Expression.Equal(propertyAccess, Expression.Constant(numericValue));
                    return equalExpression;
                }
            }

            return null;
        }

        private static Expression BuildStringFilter(Expression propertyAccess, string value)
        {
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var searchValue = Expression.Constant(value, typeof(string));
            
            var notNullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var containsCall = Expression.Call(propertyAccess, containsMethod, searchValue);
            
            return Expression.AndAlso(notNullCheck, containsCall);
        }

        private static Expression? BuildNumericFilter(Expression propertyAccess, string value, Type propertyType)
        {
            if (TryParseNumeric(value, propertyType, out var numericValue))
            {
                return Expression.Equal(propertyAccess, Expression.Constant(numericValue));
            }
            return null;
        }

        private static Expression? BuildEnumFilter(Expression propertyAccess, string value, Type propertyType)
        {
            var enumType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            
            if (Enum.TryParse(enumType, value, true, out var enumValue))
            {
                return Expression.Equal(propertyAccess, Expression.Constant(enumValue));
            }
            
            // Tentar parse por valor numérico
            if (int.TryParse(value, out var intValue) && Enum.IsDefined(enumType, intValue))
            {
                var enumVal = Enum.ToObject(enumType, intValue);
                return Expression.Equal(propertyAccess, Expression.Constant(enumVal));
            }
            
            return null;
        }

        private static Expression? BuildDateFilter(Expression propertyAccess, string value, Type propertyType)
        {
            if (DateTime.TryParse(value, out var dateValue))
            {
                // Para filtros de data, considerar apenas a data (sem hora)
                var dateOnly = dateValue.Date;
                var nextDay = dateOnly.AddDays(1);
                
                var startExpression = Expression.GreaterThanOrEqual(propertyAccess, Expression.Constant(dateOnly));
                var endExpression = Expression.LessThan(propertyAccess, Expression.Constant(nextDay));
                
                return Expression.AndAlso(startExpression, endExpression);
            }
            
            return null;
        }

        private static Expression? BuildBooleanFilter(Expression propertyAccess, string value)
        {
            if (bool.TryParse(value, out var boolValue))
            {
                return Expression.Equal(propertyAccess, Expression.Constant(boolValue));
            }
            
            // Verificar valores alternativos
            if (value.ToLower() is "1" or "sim" or "yes" or "ativo" or "true")
            {
                return Expression.Equal(propertyAccess, Expression.Constant(true));
            }
            
            if (value.ToLower() is "0" or "não" or "nao" or "no" or "inativo" or "false")
            {
                return Expression.Equal(propertyAccess, Expression.Constant(false));
            }
            
            return null;
        }

        private static bool IsNumericType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(int) || underlyingType == typeof(long) || 
                   underlyingType == typeof(decimal) || underlyingType == typeof(double) || 
                   underlyingType == typeof(float) || underlyingType == typeof(short) ||
                   underlyingType == typeof(byte);
        }

        private static bool IsEnumType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType.IsEnum;
        }

        private static bool IsDateType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset) || 
                   underlyingType == typeof(DateOnly);
        }

        private static bool TryParseNumeric(string value, Type type, out object? result)
        {
            result = null;
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            try
            {
                result = underlyingType switch
                {
                    var t when t == typeof(int) => int.Parse(value),
                    var t when t == typeof(long) => long.Parse(value),
                    var t when t == typeof(decimal) => decimal.Parse(value),
                    var t when t == typeof(double) => double.Parse(value),
                    var t when t == typeof(float) => float.Parse(value),
                    var t when t == typeof(short) => short.Parse(value),
                    var t when t == typeof(byte) => byte.Parse(value),
                    _ => null
                };
                
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsPropertyUsedInDisplay<T>(PropertyInfo property, EntityMetadataCache<T> metadata) where T : class
        {
            // Verificar se a propriedade é usada na grid ou form
            return metadata.GridProperties.Contains(property) || 
                   metadata.FormProperties.Contains(property);
        }

        #endregion
    }

    /// <summary>
    /// Extensões para paginação automática
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Aplica paginação automática
        /// </summary>
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize) where T : class
        {
            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };
        }
    }

    /// <summary>
    /// Resultado paginado
    /// </summary>
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
