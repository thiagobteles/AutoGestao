using System.Reflection;
using FGT.Atributes;
using FGT.Models;

namespace FGT.Helpers
{
    /// <summary>
    /// Helper para processar anotações [ReferenceFilter] e gerar configurações
    /// </summary>
    public static class ReferenceFilterHelper
    {
        /// <summary>
        /// Extrai configurações de filtro de uma propriedade
        /// </summary>
        public static ReferenceFilterConfig? GetFilterConfig(PropertyInfo property)
        {
            var filterAttributes = property.GetCustomAttributes<ReferenceFilterAttribute>().ToList();

            if (filterAttributes.Count == 0)
            {
                return null;
            }

            var config = new ReferenceFilterConfig
            {
                PropertyName = property.Name,
                Filters = [.. filterAttributes.Select(attr => new ReferenceFilterInfo
                {
                    FilterField = attr.FilterField,
                    FilterValue = attr.FilterValue,
                    Operator = attr.Operator,
                    IsPropertyReference = attr.IsPropertyReference
                })]
            };

            return config;
        }

        /// <summary>
        /// Resolve os valores dos filtros baseado na entidade atual
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="property">Propriedade com os filtros</param>
        /// <param name="entity">Instância da entidade</param>
        /// <returns>Dicionário com os filtros resolvidos</returns>
        public static Dictionary<string, string> ResolveFilters<T>(PropertyInfo property, T entity) where T : class
        {
            var config = GetFilterConfig(property);
            if (config == null)
            {
                return [];
            }

            var resolvedFilters = new Dictionary<string, string>();
            var entityType = typeof(T);

            foreach (var filter in config.Filters)
            {
                string? resolvedValue = null;

                if (filter.IsPropertyReference)
                {
                    // Tentar obter o valor da propriedade da entidade
                    var sourceProperty = entityType.GetProperty(filter.FilterValue);
                    if (sourceProperty != null)
                    {
                        var value = sourceProperty.GetValue(entity);
                        if (value != null)
                        {
                            resolvedValue = value.ToString();
                        }
                    }
                }
                else
                {
                    // Usar valor literal
                    resolvedValue = filter.FilterValue;
                }

                // Adicionar ao dicionário se o valor foi resolvido
                if (!string.IsNullOrWhiteSpace(resolvedValue))
                {
                    resolvedFilters[filter.FilterField] = resolvedValue;
                }
            }

            return resolvedFilters;
        }

        /// <summary>
        /// Serializa a configuração de filtros para JSON a ser usado no frontend
        /// </summary>
        public static string SerializeFilterConfig(ReferenceFilterConfig? config)
        {
            if (config == null || config.Filters.Count == 0)
            {
                return "{}";
            }

            var filterDict = new Dictionary<string, object>();
            foreach (var filter in config.Filters)
            {
                filterDict[filter.FilterField] = new
                {
                    value = filter.FilterValue,
                    isProperty = filter.IsPropertyReference,
                    @operator = filter.Operator.ToString()
                };
            }

            return System.Text.Json.JsonSerializer.Serialize(filterDict);
        }

        /// <summary>
        /// Verifica se uma propriedade tem filtros configurados
        /// </summary>
        public static bool HasFilters(PropertyInfo property)
        {
            return property.GetCustomAttributes<ReferenceFilterAttribute>().Any();
        }
    }
}