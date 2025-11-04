using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Models.Report;
using System.Reflection;

namespace AutoGestao.Services
{
    /// <summary>
    /// Serviço para inspecionar entidades e descobrir suas propriedades e relacionamentos
    /// </summary>
    public class EntityInspectorService(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Obtém informações sobre uma entidade por nome
        /// </summary>
        public EntityInfo GetEntityInfo(string entityName)
        {
            var entityType = FindEntityType(entityName) ?? throw new Exception($"Entidade '{entityName}' não encontrada");
            var formConfig = entityType.GetCustomAttribute<FormConfigAttribute>();

            return new EntityInfo
            {
                Name = entityName,
                DisplayName = formConfig?.Title ?? entityName,
                Icon = formConfig?.Icon ?? "fas fa-database",
                Properties = GetEntityProperties(entityType)
            };
        }

        /// <summary>
        /// Obtém todas as propriedades de uma entidade, incluindo propriedades navegacionais
        /// </summary>
        private List<PropertyInfoInspector> GetEntityProperties(Type entityType, string prefix = "")
        {
            var properties = new List<PropertyInfoInspector>();

            foreach (var prop in entityType.GetProperties())
            {
                // Ignorar propriedades de coleção
                if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    continue;
                }

                // Ignorar propriedades herdadas de BaseEntidade que não são úteis
                if (new[] { "Id", "IdEmpresa", "DataCadastro", "DataAlteracao", "IdUsuarioCadastro", "IdUsuarioAlteracao" }
                    .Contains(prop.Name) && string.IsNullOrEmpty(prefix))
                {
                    continue;
                }

                var fullPath = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                var formField = prop.GetCustomAttribute<FormFieldAttribute>();
                var gridField = prop.GetCustomAttribute<GridFieldAttribute>();
                var referenceText = prop.GetCustomAttribute<ReferenceTextAttribute>();

                // Adicionar a propriedade
                properties.Add(new PropertyInfoInspector
                {
                    PropertyName = fullPath,
                    Label = formField?.Name ?? gridField?.DisplayName ?? prop.Name,
                    PropertyType = GetSimpleTypeName(prop.PropertyType),
                    Section = formField?.Section ?? "Geral",
                    Icon = formField?.Icon ?? "fas fa-field",
                    IsReferenceText = referenceText != null
                });

                // Se for uma propriedade navegacional (entidade relacionada), inspecionar suas propriedades
                if (IsNavigationProperty(prop))
                {
                    var navType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    // Apenas um nível de profundidade para evitar recursão infinita
                    if (string.IsNullOrEmpty(prefix))
                    {
                        foreach (var navProp in navType.GetProperties())
                        {
                            // Apenas propriedades simples ou com [ReferenceText]
                            if (navProp.GetCustomAttribute<ReferenceTextAttribute>() != null ||
                                IsSimpleType(navProp.PropertyType))
                            {
                                var navFormField = navProp.GetCustomAttribute<FormFieldAttribute>();
                                var navFullPath = $"{prop.Name}.{navProp.Name}";

                                properties.Add(new PropertyInfoInspector
                                {
                                    PropertyName = navFullPath,
                                    Label = $"{prop.Name} - {navFormField?.Name ?? navProp.Name}",
                                    PropertyType = GetSimpleTypeName(navProp.PropertyType),
                                    Section = $"{prop.Name} (Relacionamento)",
                                    Icon = "fas fa-link",
                                    IsReferenceText = navProp.GetCustomAttribute<ReferenceTextAttribute>() != null
                                });
                            }
                        }
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// Verifica se é uma propriedade navegacional (entidade relacionada)
        /// </summary>
        private static bool IsNavigationProperty(PropertyInfo prop)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            return type.IsClass &&
                   type != typeof(string) &&
                   !type.IsValueType &&
                   type.IsSubclassOf(typeof(BaseEntidade));
        }

        /// <summary>
        /// Verifica se é um tipo simples
        /// </summary>
        private static bool IsSimpleType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType.IsPrimitive ||
                   underlyingType == typeof(string) ||
                   underlyingType == typeof(decimal) ||
                   underlyingType == typeof(DateTime) ||
                   underlyingType == typeof(DateTimeOffset) ||
                   underlyingType == typeof(TimeSpan) ||
                   underlyingType == typeof(Guid) ||
                   underlyingType.IsEnum;
        }

        /// <summary>
        /// Obtém o nome simples do tipo
        /// </summary>
        private static string GetSimpleTypeName(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (underlyingType == typeof(string)) return "text";
            if (underlyingType == typeof(int) || underlyingType == typeof(long)) return "number";
            if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float)) return "currency";
            if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset)) return "date";
            if (underlyingType == typeof(bool)) return "boolean";
            if (underlyingType.IsEnum) return "enum";

            return "text";
        }

        /// <summary>
        /// Encontra o tipo de entidade pelo nome
        /// </summary>
        private Type? FindEntityType(string entityName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
                .Where(t => t.IsClass &&
                           !t.IsAbstract &&
                           t.IsSubclassOf(typeof(BaseEntidade)) &&
                           t.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return types.FirstOrDefault();
        }

        /// <summary>
        /// Lista todas as entidades disponíveis
        /// </summary>
        public List<EntitySummary> GetAvailableEntities()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var entities = assembly.GetTypes()
                .Where(t => t.IsClass &&
                           !t.IsAbstract &&
                           t.IsSubclassOf(typeof(BaseEntidade)) &&
                           !t.Name.StartsWith("Base") &&
                           t.Namespace != null &&
                           !t.Namespace.Contains("Relatorio"))
                .Select(t => new EntitySummary
                {
                    Name = t.Name,
                    DisplayName = t.GetCustomAttribute<FormConfigAttribute>()?.Title ?? t.Name,
                    Icon = t.GetCustomAttribute<FormConfigAttribute>()?.Icon ?? "fas fa-database",
                    Namespace = t.Namespace
                })
                .OrderBy(e => e.DisplayName)
                .ToList();

            return entities;
        }
    }

    /// <summary>
    /// Informações sobre uma entidade
    /// </summary>
    public class EntityInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<PropertyInfoInspector> Properties { get; set; } = [];
    }

    /// <summary>
    /// Informações sobre uma propriedade
    /// </summary>
    public class PropertyInfoInspector
    {
        public string PropertyName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsReferenceText { get; set; }
    }

    /// <summary>
    /// Resumo de entidade
    /// </summary>
    public class EntitySummary
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Namespace { get; set; }
    }
}
