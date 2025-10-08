using AutoGestao.Data;
using AutoGestao.Helpers;
using AutoGestao.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoGestao.Services
{
    /// <summary>
    /// Serviço genérico para ReferenceController
    /// Usa GridColumnBuilder para extrair metadados das anotações
    /// </summary>
    public class GenericReferenceService(ApplicationDbContext context, ILogger<GenericReferenceService> logger)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<GenericReferenceService> _logger = logger;

        // Cache de metadados para performance
        private static readonly Dictionary<Type, ReferenceMetadata> _metadataCache = [];

        /// <summary>
        /// Busca um item por ID de forma genérica
        /// </summary>
        public async Task<ReferenceItem?> GetByIdAsync<T>(string id) where T : class
        {
            if (!long.TryParse(id, out var entityId))
            {
                return null;
            }

            var metadata = GetMetadata<T>();
            var query = _context.Set<T>().AsQueryable();

            // Aplicar Includes necessários
            query = ApplyIncludes(query, metadata);

            // Buscar pela propriedade Id
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, idProperty);
            var constant = Expression.Constant(entityId);
            var equals = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            var entity = await query.FirstOrDefaultAsync(lambda);
            return entity == null 
                ? null
                : BuildReferenceItem(entity, metadata);
        }

        /// <summary>
        /// Busca itens usando termo de pesquisa
        /// </summary>
        public async Task<List<ReferenceItem>> SearchAsync<T>(string searchTerm, int pageSize, Dictionary<string, string>? filters = null) where T : class
        {
            var metadata = GetMetadata<T>();
            var query = _context.Set<T>().AsQueryable();

            // Aplicar Includes
            query = ApplyIncludes(query, metadata);

            // Aplicar filtro de busca
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = ApplySearchFilter(query, searchTerm, metadata);
            }

            // Aplicar filtros dinâmicos
            if (filters != null && filters.Any())
            {
                query = ApplyDynamicFilters(query, filters);
            }

            // Buscar e converter
            var entities = await query.Take(pageSize).ToListAsync();
            return [.. entities.Select(e => BuildReferenceItem(e, metadata))];
        }

        /// <summary>
        /// Obtém metadados usando GridColumnBuilder (com cache)
        /// </summary>
        private static ReferenceMetadata GetMetadata<T>() where T : class
        {
            var type = typeof(T);

            if (_metadataCache.TryGetValue(type, out var cached))
            {
                return cached;
            }

            var metadata = GridColumnBuilder.GetReferenceMetadata<T>();
            _metadataCache[type] = metadata;
            return metadata;
        }

        /// <summary>
        /// Constrói ReferenceItem a partir da entidade
        /// </summary>
        private static ReferenceItem BuildReferenceItem<T>(T entity, ReferenceMetadata metadata)
        {
            var item = new ReferenceItem();

            // Id
            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null)
            {
                var idValue = idProp.GetValue(entity);
                item.Value = idValue?.ToString() ?? "";
            }

            // Text
            if (metadata.TextProperty != null)
            {
                var textValue = GetPropertyValue(entity, metadata.TextProperty);
                item.Text = textValue?.ToString() ?? "";
            }

            // Subtitle (concatena múltiplas propriedades)
            var subtitleParts = new List<string>();
            foreach (var subtitleProp in metadata.SubtitleProperties)
            {
                var value = GetPropertyValue(entity, subtitleProp);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    var formatted = FormatValue(value, subtitleProp.Format);
                    var part = string.IsNullOrEmpty(subtitleProp.Prefix)
                        ? formatted
                        : $"{subtitleProp.Prefix}{formatted}";
                    subtitleParts.Add(part);
                }
            }

            item.Subtitle = subtitleParts.Any() ? string.Join(" • ", subtitleParts) : null;

            return item;
        }

        /// <summary>
        /// Obtém valor de propriedade, navegando por relacionamentos se necessário
        /// </summary>
        private static object? GetPropertyValue(object entity, ReferencePropertyInfo propInfo)
        {
            if (string.IsNullOrEmpty(propInfo.NavigationPath))
            {
                return propInfo.Property.GetValue(entity);
            }

            // Navegar por relacionamentos (ex: VeiculoMarca.Descricao)
            var parts = propInfo.NavigationPath.Split('.');
            object? current = entity;

            foreach (var part in parts)
            {
                if (current == null)
                {
                    return null;
                }

                var prop = current.GetType().GetProperty(part);
                if (prop == null)
                {
                    return null;
                }

                current = prop.GetValue(current);
            }

            return current;
        }

        /// <summary>
        /// Formata valor usando máscara se fornecida
        /// </summary>
        private static string FormatValue(object value, string? format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString() ?? "";
            }

            var str = value.ToString() ?? "";

            return format switch
            {
                "###.###.###-##" when str.Length == 11
                    => $"{str[..3]}.{str.Substring(3, 3)}.{str.Substring(6, 3)}-{str.Substring(9, 2)}",

                "##.###.###/####-##" when str.Length == 14
                    => $"{str[..2]}.{str.Substring(2, 3)}.{str.Substring(5, 3)}/{str.Substring(8, 4)}-{str.Substring(12, 2)}",

                "(##) ####-####" when str.Length == 10
                    => $"({str[..2]}) {str.Substring(2, 4)}-{str.Substring(6, 4)}",

                "(##) #####-####" when str.Length == 11
                    => $"({str[..2]}) {str.Substring(2, 5)}-{str.Substring(7, 4)}",

                _ => str
            };
        }

        /// <summary>
        /// Aplica Includes necessários
        /// </summary>
        private static IQueryable<T> ApplyIncludes<T>(IQueryable<T> query, ReferenceMetadata metadata) where T : class
        {
            var includes = new HashSet<string>();

            // Coletar todos os includes
            void AddIncludeFromPath(string? path)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var firstPart = path.Split('.')[0];
                    includes.Add(firstPart);
                }
            }

            AddIncludeFromPath(metadata.TextProperty?.NavigationPath);

            foreach (var subtitle in metadata.SubtitleProperties)
            {
                AddIncludeFromPath(subtitle.NavigationPath);
            }

            foreach (var searchable in metadata.SearchableProperties)
            {
                AddIncludeFromPath(searchable.NavigationPath);
            }

            // Aplicar includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        /// <summary>
        /// Aplica filtro de busca em campos searchable
        /// </summary>
        private static IQueryable<T> ApplySearchFilter<T>(IQueryable<T> query, string searchTerm, ReferenceMetadata metadata) where T : class
        {
            var termLower = searchTerm.ToLower();
            var parameter = Expression.Parameter(typeof(T), "e");
            Expression? filterExpression = null;

            foreach (var searchProp in metadata.SearchableProperties)
            {
                var property = Expression.Property(parameter, searchProp.Property);

                // String operations
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);

                if (toLowerMethod != null && containsMethod != null)
                {
                    var toLower = Expression.Call(property, toLowerMethod);
                    var constant = Expression.Constant(termLower);
                    var contains = Expression.Call(toLower, containsMethod, constant);
                    var nullCheck = Expression.NotEqual(property, Expression.Constant(null));
                    var condition = Expression.AndAlso(nullCheck, contains);

                    filterExpression = filterExpression == null
                        ? condition
                        : Expression.OrElse(filterExpression, condition);
                }
            }

            if (filterExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Aplica filtros dinâmicos com suporte a propriedades nullable
        /// VERSÃO SIMPLIFICADA E ROBUSTA
        /// </summary>
        private static IQueryable<T> ApplyDynamicFilters<T>(IQueryable<T> query, Dictionary<string, string> filters) where T : class
        {
            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.Key);
                if (property == null)
                {
                    continue;
                }

                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, property);

                // Obter o tipo da propriedade (pode ser nullable)
                var propertyType = property.PropertyType;

                // Converter o valor do filtro para o tipo correto
                object? convertedValue = ConvertFilterValue(filter.Value, propertyType);
                if (convertedValue == null)
                {
                    continue;
                }

                // Criar constante com o tipo exato da propriedade
                var constant = Expression.Constant(convertedValue, propertyType);

                // Criar expressão de igualdade
                var equals = Expression.Equal(propertyAccess, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Converte valor string para o tipo apropriado (incluindo nullable)
        /// </summary>
        private static object? ConvertFilterValue(string value, Type targetType)
        {
            try
            {
                // Se for nullable, obter o tipo base
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                var isNullable = underlyingType != null;
                var typeToConvert = underlyingType ?? targetType;

                // Converter para o tipo base
                object convertedValue;

                if (typeToConvert == typeof(int))
                {
                    convertedValue = int.Parse(value);
                }
                else if (typeToConvert == typeof(long))
                {
                    convertedValue = long.Parse(value);
                }
                else if (typeToConvert == typeof(bool))
                {
                    convertedValue = bool.Parse(value);
                }
                else if (typeToConvert == typeof(DateTime))
                {
                    convertedValue = DateTime.Parse(value);
                }
                else if (typeToConvert == typeof(TimeSpan))
                {
                    convertedValue = TimeSpan.Parse(value);
                }
                else if (typeToConvert == typeof(decimal))
                {
                    convertedValue = decimal.Parse(value);
                }
                else if (typeToConvert == typeof(double))
                {
                    convertedValue = double.Parse(value);
                }
                else if (typeToConvert == typeof(float))
                {
                    convertedValue = float.Parse(value);
                }
                else if (typeToConvert == typeof(Guid))
                {
                    convertedValue = Guid.Parse(value);
                }
                else
                {
                    convertedValue = value; // Fallback: retornar string
                }

                // Se a propriedade é nullable, converter para Nullable<T>
                if (isNullable)
                {
                    var nullableType = typeof(Nullable<>).MakeGenericType(typeToConvert);
                    return Activator.CreateInstance(nullableType, convertedValue);
                }

                return convertedValue;
            }
            catch (Exception ex)
            {
                // Log para debug (remover em produção se necessário)
                System.Diagnostics.Debug.WriteLine($"Erro ao converter '{value}' para {targetType.Name}: {ex.Message}");
                return null;
            }
        }
    }
}