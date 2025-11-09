using AutoGestao.Data;
using AutoGestao.Helpers;
using AutoGestao.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoGestao.Services
{
    public class GenericReferenceService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<GenericReferenceService> logger)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<GenericReferenceService> _logger = logger;
        private static readonly Dictionary<Type, ReferenceMetadata> _metadataCache = [];

        public async Task<ReferenceItem?> GetByIdAsync<T>(string id) where T : class
        {
            if (!long.TryParse(id, out var entityId))
            {
                return null;
            }

            // 🔧 FIX: Setar CurrentEmpresaId no contexto para Query Filter Global
            _context.CurrentEmpresaId = GetCurrentEmpresaId();

            var metadata = GetMetadata<T>();
            var query = _context.Set<T>().AsQueryable();
            query = ApplyIncludes(query, metadata);

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
            return entity == null ? null : BuildReferenceItem(entity, metadata);
        }

        public async Task<List<ReferenceItem>> SearchAsync<T>(string searchTerm, int pageSize, Dictionary<string, string>? filters = null) where T : class
        {
            // 🔧 FIX: Setar CurrentEmpresaId no contexto para Query Filter Global
            _context.CurrentEmpresaId = GetCurrentEmpresaId();

            var metadata = GetMetadata<T>();
            var query = _context.Set<T>().AsQueryable();
            query = ApplyIncludes(query, metadata);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = ApplySearchFilter(query, searchTerm, metadata);
            }

            if (filters != null && filters.Count != 0)
            {
                query = ApplyDynamicFilters(query, filters);
            }

            var entities = await query.Take(pageSize).ToListAsync();
            return [.. entities.Select(e => BuildReferenceItem(e, metadata))];
        }

        private ReferenceMetadata GetMetadata<T>() where T : class
        {
            var type = typeof(T);
            if (_metadataCache.TryGetValue(type, out var cached))
            {
                return cached;
            }

            var metadata = GridColumnBuilder.GetReferenceMetadata<T>();
            _metadataCache[type] = metadata;

            // 📊 Log para debug
            _logger.LogInformation(
                "🔍 Metadata carregada para {Type}: Text={Text}, Searchable={Searchable}, Subtitle={Subtitle}",
                type.Name,
                metadata.TextProperty?.Property.Name ?? "N/A",
                string.Join(", ", metadata.SearchableProperties.Select(p => p.Property.Name)),
                string.Join(", ", metadata.SubtitleProperties.Select(p => p.Property.Name))
            );

            return metadata;
        }

        private static ReferenceItem BuildReferenceItem<T>(T entity, ReferenceMetadata metadata)
        {
            var item = new ReferenceItem();

            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null)
            {
                var idValue = idProp.GetValue(entity);
                item.Value = idValue?.ToString() ?? "";
            }

            if (metadata.TextProperty != null)
            {
                var textValue = GetPropertyValue(entity, metadata.TextProperty);
                item.Text = textValue?.ToString() ?? "";
            }

            var subtitleParts = new List<string>();
            foreach (var subtitleProp in metadata.SubtitleProperties)
            {
                var value = GetPropertyValue(entity, subtitleProp);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    var formatted = FormatValue(value, subtitleProp.Format);
                    var part = string.IsNullOrEmpty(subtitleProp.Prefix) ? formatted : $"{subtitleProp.Prefix}{formatted}";
                    subtitleParts.Add(part);
                }
            }

            item.Subtitle = subtitleParts.Any() ? string.Join(" | ", subtitleParts) : null;
            return item;
        }

        private static object? GetPropertyValue<T>(T entity, ReferencePropertyInfo propertyInfo)
        {
            if (string.IsNullOrEmpty(propertyInfo.NavigationPath))
            {
                return propertyInfo.Property.GetValue(entity);
            }

            var parts = propertyInfo.NavigationPath.Split('.');
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

        private static string FormatValue(object value, string? format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString() ?? "";
            }

            var str = value.ToString() ?? "";

            return format switch
            {
                "###.###.###-##" when str.Length == 11 => $"{str[..3]}.{str.Substring(3, 3)}.{str.Substring(6, 3)}-{str.Substring(9, 2)}",
                "##.###.###/####-##" when str.Length == 14 => $"{str[..2]}.{str.Substring(2, 3)}.{str.Substring(5, 3)}/{str.Substring(8, 4)}-{str.Substring(12, 2)}",
                "(##) ####-####" when str.Length == 10 => $"({str[..2]}) {str.Substring(2, 4)}-{str.Substring(6, 4)}",
                "(##) #####-####" when str.Length == 11 => $"({str[..2]}) {str.Substring(2, 5)}-{str.Substring(7, 4)}",
                _ => str
            };
        }

        private static IQueryable<T> ApplyIncludes<T>(IQueryable<T> query, ReferenceMetadata metadata) where T : class
        {
            var includes = new HashSet<string>();

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

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        private static IQueryable<T> ApplySearchFilter<T>(IQueryable<T> query, string searchTerm, ReferenceMetadata metadata) where T : class
        {
            var termLower = searchTerm.ToLower();
            var termNormalized = NormalizeSearchTerm(searchTerm); // Remove pontuação
            var parameter = Expression.Parameter(typeof(T), "e");
            Expression? filterExpression = null;

            foreach (var searchProp in metadata.SearchableProperties)
            {
                // 🔧 FIX: Construir expressão considerando NavigationPath
                Expression property = parameter;

                // Se tem NavigationPath (ex: "Empresa.Nome"), navegar pelas propriedades
                if (!string.IsNullOrEmpty(searchProp.NavigationPath))
                {
                    var parts = searchProp.NavigationPath.Split('.');
                    foreach (var part in parts)
                    {
                        property = Expression.Property(property, part);
                    }
                }
                else
                {
                    // Propriedade direta na entidade
                    property = Expression.Property(parameter, searchProp.Property);
                }

                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                var replaceMethod = typeof(string).GetMethod("Replace", [typeof(string), typeof(string)]);

                if (toLowerMethod != null && containsMethod != null)
                {
                    // 🔧 FIX: Adicionar null check para propriedades de navegação
                    Expression? nullCheckExpression = null;

                    // Verificar null em toda a cadeia de navegação
                    if (!string.IsNullOrEmpty(searchProp.NavigationPath))
                    {
                        var navProperty = (Expression)parameter;
                        var parts = searchProp.NavigationPath.Split('.');

                        foreach (var part in parts.Take(parts.Length))
                        {
                            navProperty = Expression.Property(navProperty, part);
                            var navNullCheck = Expression.NotEqual(navProperty, Expression.Constant(null));
                            nullCheckExpression = nullCheckExpression == null
                                ? navNullCheck
                                : Expression.AndAlso(nullCheckExpression, navNullCheck);
                        }
                    }
                    else
                    {
                        nullCheckExpression = Expression.NotEqual(property, Expression.Constant(null));
                    }

                    Expression searchExpression;

                    // ✅ NOVO: Se o campo tem Format (CNPJ, CPF, telefone), aplicar replace para normalizar
                    if (HasFormatMask(searchProp.Format))
                    {
                        // Remover pontuação do campo do banco antes de comparar
                        var normalizedProperty = ApplyNormalizationReplaces(property, replaceMethod!);
                        var normalizedConstant = Expression.Constant(termNormalized);
                        searchExpression = Expression.Call(normalizedProperty, containsMethod, normalizedConstant);
                    }
                    else
                    {
                        // Busca normal com ToLower
                        var toLower = Expression.Call(property, toLowerMethod);
                        var constant = Expression.Constant(termLower);
                        searchExpression = Expression.Call(toLower, containsMethod, constant);
                    }

                    var condition = Expression.AndAlso(nullCheckExpression, searchExpression);
                    filterExpression = filterExpression == null ? condition : Expression.OrElse(filterExpression, condition);
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
        /// Normaliza o termo de busca removendo caracteres de pontuação
        /// </summary>
        private static string NormalizeSearchTerm(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return term;
            }

            // Remove caracteres comuns de formatação
            return term.Replace(".", "")
                      .Replace("-", "")
                      .Replace("/", "")
                      .Replace("(", "")
                      .Replace(")", "")
                      .Replace(" ", "")
                      .ToLower();
        }

        /// <summary>
        /// Verifica se o formato indica um campo com máscara (CNPJ, CPF, telefone, etc)
        /// </summary>
        private static bool HasFormatMask(string? format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return false;
            }

            // Formatos conhecidos que usam pontuação
            return format.Contains(".") ||
                   format.Contains("-") ||
                   format.Contains("/") ||
                   format.Contains("(") ||
                   format.Contains(")");
        }

        /// <summary>
        /// Aplica múltiplos replaces para remover caracteres de formatação do campo
        /// </summary>
        private static Expression ApplyNormalizationReplaces(Expression property, MethodInfo replaceMethod)
        {
            // Cadeia de replaces: campo.Replace(".", "").Replace("-", "").Replace("/", "")...
            Expression result = property;

            var charsToRemove = new[] { ".", "-", "/", "(", ")", " " };

            foreach (var charToRemove in charsToRemove)
            {
                var searchChar = Expression.Constant(charToRemove);
                var emptyString = Expression.Constant("");
                result = Expression.Call(result, replaceMethod, searchChar, emptyString);
            }

            return result;
        }

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
                var propertyType = property.PropertyType;

                var convertedValue = ConvertFilterValue(filter.Value, propertyType);
                if (convertedValue == null)
                {
                    continue;
                }

                var constant = Expression.Constant(convertedValue, propertyType);
                var equals = Expression.Equal(propertyAccess, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

                query = query.Where(lambda);
            }

            return query;
        }

        private static object? ConvertFilterValue(string value, Type targetType)
        {
            try
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                var isNullable = underlyingType != null;
                var typeToConvert = underlyingType ?? targetType;

                if (string.IsNullOrWhiteSpace(value))
                {
                    return isNullable ? null : throw new InvalidOperationException("Cannot convert empty string to non-nullable type");
                }

                if (typeToConvert == typeof(string))
                {
                    return value;
                }

                if (typeToConvert == typeof(int))
                {
                    return int.Parse(value);
                }

                if (typeToConvert == typeof(long))
                {
                    return long.Parse(value);
                }

                if (typeToConvert == typeof(decimal))
                {
                    return decimal.Parse(value);
                }

                if (typeToConvert == typeof(double))
                {
                    return double.Parse(value);
                }

                if (typeToConvert == typeof(float))
                {
                    return float.Parse(value);
                }

                if (typeToConvert == typeof(bool))
                {
                    return bool.Parse(value);
                }

                if (typeToConvert == typeof(DateTime))
                {
                    return DateTime.Parse(value);
                }

                if (typeToConvert == typeof(Guid))
                {
                    return Guid.Parse(value);
                }

                if (typeToConvert.IsEnum)
                {
                    return Enum.Parse(typeToConvert, value);
                }

                return Convert.ChangeType(value, typeToConvert);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Obtém o ID da empresa do usuário logado
        /// </summary>
        private long GetCurrentEmpresaId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User == null)
            {
                _logger.LogWarning("HttpContext ou User não disponível, retornando empresaId = 1");
                return 1;
            }

            var empresaIdClaim = httpContext.User.FindFirst("EmpresaId")?.Value;
            if (long.TryParse(empresaIdClaim, out var empresaId))
            {
                return empresaId;
            }

            _logger.LogWarning("Claim 'EmpresaId' não encontrado ou inválido, retornando empresaId = 1");
            return 1;
        }
    }
}