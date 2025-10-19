using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using System.Collections.Concurrent;
using System.Reflection;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Cache global de metadados para múltiplas entidades
    /// </summary>
    public static class GlobalEntityMetadataCache
    {
        private static readonly ConcurrentDictionary<Type, object> _caches = new();

        /// <summary>
        /// Obtém cache de metadados para um tipo específico
        /// </summary>
        public static EntityMetadataCache<T> GetCache<T>() where T : class
        {
            return (EntityMetadataCache<T>)_caches.GetOrAdd(typeof(T), _ => EntityMetadataCache<T>.Instance);
        }

        /// <summary>
        /// Limpa cache de um tipo específico
        /// </summary>
        public static void ClearCache<T>() where T : class
        {
            _caches.TryRemove(typeof(T), out _);
        }

        /// <summary>
        /// Limpa todos os caches
        /// </summary>
        public static void ClearAll()
        {
            _caches.Clear();
        }

        /// <summary>
        /// Obtém estatísticas dos caches
        /// </summary>
        public static Dictionary<string, int> GetCacheStatistics()
        {
            return _caches.ToDictionary(
                kvp => kvp.Key.Name,
                kvp => 1 // Cache count - sempre 1 por tipo
            );
        }

        /// <summary>
        /// Warm-up de todos os caches principais
        /// </summary>
        public static async Task WarmUpAllCachesAsync()
        {
            await Task.Run(() =>
            {
                // Entidades principais do sistema
                var entityTypes = GetEntityTypes();

                Parallel.ForEach(entityTypes, entityType =>
                {
                    try
                    {
                        var method = typeof(GlobalEntityMetadataCache)
                            .GetMethod(nameof(GetCache))
                            ?.MakeGenericMethod(entityType);

                        method?.Invoke(null, null);
                    }
                    catch (Exception ex)
                    {
                        // Log do erro mas não falha o warm-up
                        Console.WriteLine($"Erro ao fazer warm-up de {entityType.Name}: {ex.Message}");
                    }
                });
            });
        }

        /// <summary>
        /// Obtém tipos de entidades automaticamente
        /// </summary>
        private static List<Type> GetEntityTypes()
        {
            try
            {
                return Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass &&
                               !t.IsAbstract &&
                               t.Namespace?.StartsWith("AutoGestao.Entidades") == true)
                    .ToList();
            }
            catch
            {
                // Se não conseguir descobrir automaticamente, retornar lista vazia
                return [];
            }
        }
    }

    /// <summary>
    /// Cache de metadados da entidade para performance ultra-otimizada
    /// Elimina reflexão repetitiva e melhora drasticamente a performance
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    public class EntityMetadataCache<T> where T : class
    {
        private static readonly Lazy<EntityMetadataCache<T>> _instance =
            new(() => new EntityMetadataCache<T>());

        public static EntityMetadataCache<T> Instance => _instance.Value;

        // Caches para diferentes tipos de propriedades
        private readonly Lazy<List<PropertyInfo>> _allProperties;
        private readonly Lazy<List<PropertyInfo>> _gridProperties;
        private readonly Lazy<List<PropertyInfo>> _formProperties;
        private readonly Lazy<List<PropertyInfo>> _enumProperties;
        private readonly Lazy<List<PropertyInfo>> _dateProperties;
        private readonly Lazy<List<PropertyInfo>> _referenceProperties;
        private readonly Lazy<List<PropertyInfo>> _searchableProperties;
        private readonly Lazy<List<PropertyInfo>> _navigationProperties;
        private readonly Lazy<FormConfigAttribute?> _formConfig;
        private readonly Lazy<FormTabsAttribute?> _formTabs;
        private readonly Lazy<string> _defaultOrderField;
        private readonly Lazy<Dictionary<string, PropertyInfo>> _propertyLookup;

        private EntityMetadataCache()
        {
            _allProperties = new Lazy<List<PropertyInfo>>(() =>
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList());

            _gridProperties = new Lazy<List<PropertyInfo>>(() =>
                _allProperties.Value.Where(HasGridAttribute).ToList());

            _formProperties = new Lazy<List<PropertyInfo>>(() =>
                _allProperties.Value.Where(HasFormFieldAttribute).ToList());

            _enumProperties = new Lazy<List<PropertyInfo>>(() =>
                _allProperties.Value.Where(IsEnumProperty).ToList());

            _dateProperties = new Lazy<List<PropertyInfo>>(() =>
                _allProperties.Value.Where(IsDateProperty).ToList());

            _referenceProperties = new Lazy<List<PropertyInfo>>(() =>
                _allProperties.Value.Where(IsReferenceProperty).ToList());

            _searchableProperties = new Lazy<List<PropertyInfo>>(() =>
                _allProperties.Value.Where(IsSearchableProperty).ToList());

            _navigationProperties = new Lazy<List<PropertyInfo>>(() =>
                _allProperties.Value.Where(IsNavigationProperty).ToList());

            _formConfig = new Lazy<FormConfigAttribute?>(() =>
                typeof(T).GetCustomAttribute<FormConfigAttribute>());

            _formTabs = new Lazy<FormTabsAttribute?>(() =>
                typeof(T).GetCustomAttribute<FormTabsAttribute>());

            _defaultOrderField = new Lazy<string>(() =>
                GetDefaultOrderField());

            _propertyLookup = new Lazy<Dictionary<string, PropertyInfo>>(() =>
                _allProperties.Value.ToDictionary(p => p.Name.ToLowerInvariant(), p => p));
        }

        #region Public Properties

        public List<PropertyInfo> AllProperties => _allProperties.Value;
        public List<PropertyInfo> GridProperties => _gridProperties.Value;
        public List<PropertyInfo> FormProperties => _formProperties.Value;
        public List<PropertyInfo> EnumProperties => _enumProperties.Value;
        public List<PropertyInfo> DateProperties => _dateProperties.Value;
        public List<PropertyInfo> ReferenceProperties => _referenceProperties.Value;
        public List<PropertyInfo> SearchableProperties => _searchableProperties.Value;
        public List<PropertyInfo> NavigationProperties => _navigationProperties.Value;
        public FormConfigAttribute? FormConfig => _formConfig.Value;
        public FormTabsAttribute? FormTabs => _formTabs.Value;
        public string DefaultOrderField => _defaultOrderField.Value;

        #endregion

        #region Public Methods

        /// <summary>
        /// Obtém propriedade por nome (case-insensitive)
        /// </summary>
        public PropertyInfo? GetPropertyByName(string name)
        {
            return _propertyLookup.Value.TryGetValue(name.ToLowerInvariant(), out var prop) ? prop : null;
        }

        /// <summary>
        /// Verifica se a entidade tem tabs configuradas
        /// </summary>
        public bool HasTabs => FormTabs?.EnableTabs == true;

        /// <summary>
        /// Obtém todas as tabs da entidade
        /// </summary>
        public List<FormTabAttribute> GetTabs()
        {
            return typeof(T).GetCustomAttributes<FormTabAttribute>()
                .OrderBy(t => t.Order)
                .ToList();
        }

        /// <summary>
        /// Obtém configuração de campo de formulário
        /// </summary>
        public FormFieldAttribute? GetFormFieldConfig(string propertyName)
        {
            var property = GetPropertyByName(propertyName);
            return property?.GetCustomAttribute<FormFieldAttribute>();
        }

        /// <summary>
        /// Obtém configuração de grid
        /// </summary>
        public object? GetGridConfig(string propertyName)
        {
            var property = GetPropertyByName(propertyName);
            return property?.GetGridAttribute();
        }

        /// <summary>
        /// Verifica se propriedade é obrigatória
        /// </summary>
        public bool IsPropertyRequired(string propertyName)
        {
            var property = GetPropertyByName(propertyName);
            if (property == null)
            {
                return false;
            }

            var formField = property.GetCustomAttribute<FormFieldAttribute>();
            return formField?.Required == true ||
                   property.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>() != null;
        }

        /// <summary>
        /// Obtém tipo de campo de formulário
        /// </summary>
        public EnumFieldType GetFieldType(string propertyName)
        {
            var property = GetPropertyByName(propertyName);
            if (property == null)
            {
                return EnumFieldType.Text;
            }

            var formField = property.GetCustomAttribute<FormFieldAttribute>();
            if (formField != null)
            {
                return formField.Type;
            }

            // Auto-detect por tipo
            return AutoDetectFieldType(property);
        }

        /// <summary>
        /// Obtém display name da propriedade
        /// </summary>
        public string GetDisplayName(string propertyName)
        {
            var property = GetPropertyByName(propertyName);
            if (property == null)
            {
                return propertyName;
            }

            return property.GetDisplayName() ?? propertyName;
        }

        #endregion

        #region Private Methods

        private static bool HasGridAttribute(PropertyInfo property)
        {
            return property.GetCustomAttributes().Any(attr =>
                attr.GetType().Name.StartsWith("Grid") ||
                attr.GetType().Namespace?.Contains("Grid") == true);
        }

        private static bool HasFormFieldAttribute(PropertyInfo property)
        {
            return property.GetCustomAttribute<FormFieldAttribute>() != null;
        }

        private static bool IsEnumProperty(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return type.IsEnum;
        }

        private static bool IsDateProperty(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly);
        }

        private static bool IsReferenceProperty(PropertyInfo property)
        {
            var formField = property.GetCustomAttribute<FormFieldAttribute>();
            return formField?.Type == EnumFieldType.Reference ||
                   (property.PropertyType.IsClass &&
                    property.PropertyType != typeof(string) &&
                    !property.PropertyType.IsArray);
        }

        private static bool IsSearchableProperty(PropertyInfo property)
        {
            return property.GetCustomAttribute<ReferenceSearchableAttribute>() != null ||
                   property.PropertyType == typeof(string) ||
                   IsNumericProperty(property);
        }

        private static bool IsNavigationProperty(PropertyInfo property)
        {
            // Verificações mais rigorosas para navegações EF
            return property.PropertyType.IsClass &&
                   property.PropertyType != typeof(string) &&
                   !property.PropertyType.IsArray &&
                   !property.PropertyType.IsEnum &&
                   property.PropertyType.Namespace?.StartsWith("AutoGestao.Entidades") == true &&
                   property.GetGetMethod()?.IsVirtual == true &&
                   !property.GetGetMethod()?.IsFinal == true &&
                   // Excluir propriedades conhecidas que não são navegações EF válidas
                   !new[] { "CriadoPorUsuario", "AlteradoPorUsuario", "Empresa" }.Contains(property.Name);
        }

        private static bool IsNumericProperty(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return type == typeof(int) || type == typeof(long) || type == typeof(decimal) ||
                   type == typeof(double) || type == typeof(float);
        }

        private string GetDefaultOrderField()
        {
            // Procurar campo marcado como GridId
            var idField = _allProperties.Value.FirstOrDefault(p =>
                p.GetCustomAttribute<GridIdAttribute>() != null);

            if (idField != null)
            {
                return idField.Name;
            }

            // Procurar campo "Codigo"
            var codigoField = _allProperties.Value.FirstOrDefault(p =>
                p.Name.Equals("Codigo", StringComparison.OrdinalIgnoreCase));

            if (codigoField != null)
            {
                return codigoField.Name;
            }

            // Procurar campo "Nome"
            var nomeField = _allProperties.Value.FirstOrDefault(p =>
                p.Name.Equals("Nome", StringComparison.OrdinalIgnoreCase));

            if (nomeField != null)
            {
                return nomeField.Name;
            }

            // Fallback para "Id"
            return "Id";
        }

        private static EnumFieldType AutoDetectFieldType(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            return type switch
            {
                var t when t.IsEnum => EnumFieldType.Select,
                var t when t == typeof(bool) => EnumFieldType.Checkbox,
                var t when t == typeof(DateTime) => EnumFieldType.Date,
                var t when t == typeof(decimal) || t == typeof(double) || t == typeof(float) => EnumFieldType.Number,
                var t when t == typeof(int) || t == typeof(long) => EnumFieldType.Number,
                var t when t == typeof(string) && property.Name.ToLower().Contains("email") => EnumFieldType.Email,
                var t when t == typeof(string) && property.Name.ToLower().Contains("telefone") => EnumFieldType.Telefone,
                var t when t == typeof(string) && property.Name.ToLower().Contains("cep") => EnumFieldType.Cep,
                var t when t == typeof(string) && property.Name.ToLower().Contains("cpf") => EnumFieldType.Cpf,
                var t when t == typeof(string) && property.Name.ToLower().Contains("cnpj") => EnumFieldType.Cnpj,
                var t when t == typeof(string) && property.Name.ToLower().Contains("url") => EnumFieldType.Url,
                var t when t == typeof(string) && property.Name.ToLower().Contains("observ") => EnumFieldType.TextArea,
                var t when t.IsClass && t != typeof(string) => EnumFieldType.Reference,
                _ => EnumFieldType.Text
            };
        }

        #endregion
    }
}