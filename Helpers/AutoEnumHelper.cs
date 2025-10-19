using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Helper Ultra-Automático para Enums
    /// Elimina necessidade de configurações manuais para enums
    /// Detecta automaticamente todos os enums e suas configurações
    /// </summary>
    public static class AutoEnumHelper
    {
        private static readonly ConcurrentDictionary<Type, List<SelectListItem>> _enumOptionsCache = new();
        private static readonly ConcurrentDictionary<string, object> _enumInfoCache = new();
        private static readonly Lazy<List<Type>> _allEnumTypes = new(DiscoverAllEnumTypes);

        #region Auto-Discovery de Enums

        /// <summary>
        /// Descobre todos os enums do assembly automaticamente
        /// </summary>
        private static List<Type> DiscoverAllEnumTypes()
        {
            return [.. Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsEnum && t.Namespace?.StartsWith("AutoGestao.Enumerador") == true)];
        }

        /// <summary>
        /// Obtém todos os enums disponíveis
        /// </summary>
        public static List<Type> GetAllEnumTypes()
        {
            return _allEnumTypes.Value;
        }

        /// <summary>
        /// Obtém opções para um enum específico com cache automático
        /// </summary>
        public static List<SelectListItem> GetEnumOptions<T>() where T : Enum
        {
            return GetEnumOptions(typeof(T));
        }

        /// <summary>
        /// Obtém opções para um enum por tipo
        /// </summary>
        public static List<SelectListItem> GetEnumOptions(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                return [];
            }

            return _enumOptionsCache.GetOrAdd(enumType, type =>
            {
                var options = new List<SelectListItem>();
                var values = Enum.GetValues(type);

                foreach (var value in values)
                {
                    var enumValue = (Enum)value;
                    var item = new SelectListItem
                    {
                        Value = ((int)value).ToString(),
                        Text = GetEnumDisplayText(enumValue),
                        Selected = false
                    };

                    // Adicionar data attributes automáticos
                    var description = GetEnumDescription(enumValue);
                    if (!string.IsNullOrEmpty(description))
                    {
                        // Usar dicionário para data attributes se necessário
                    }

                    options.Add(item);
                }

                return [.. options.OrderBy(o => o.Text)];
            });
        }

        /// <summary>
        /// Obtém opções de enum para uma propriedade específica
        /// </summary>
        public static List<SelectListItem> GetEnumOptionsForProperty(Type entityType, string propertyName)
        {
            var property = entityType.GetProperty(propertyName);
            if (property == null)
            {
                return [];
            }

            var enumType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (!enumType.IsEnum)
            {
                return [];
            }

            return GetEnumOptions(enumType);
        }

        /// <summary>
        /// Obtém informações completas de um enum
        /// </summary>
        public static EnumInfo GetEnumInfo(Type enumType)
        {
            var cacheKey = $"info_{enumType.FullName}";
            
            return (EnumInfo)_enumInfoCache.GetOrAdd(cacheKey, _ =>
            {
                if (!enumType.IsEnum)
                {
                    return new EnumInfo { Name = enumType.Name, IsEnum = false };
                }

                var values = Enum.GetValues(enumType);
                var enumInfo = new EnumInfo
                {
                    Name = enumType.Name,
                    FullName = enumType.FullName,
                    IsEnum = true,
                    Values = []
                };

                foreach (var value in values)
                {
                    var enumValue = (Enum)value;
                    enumInfo.Values.Add(new EnumValueInfo
                    {
                        Name = value.ToString(),
                        Value = (int)value,
                        DisplayText = GetEnumDisplayText(enumValue),
                        Description = GetEnumDescription(enumValue),
                        Category = GetEnumCategory(enumValue),
                        Icon = GetEnumIcon(enumValue),
                        CssClass = GetEnumCssClass(enumValue),
                        IsActive = GetEnumIsActive(enumValue)
                    });
                }

                return enumInfo;
            });
        }

        #endregion

        #region Auto-Detection de Propriedades do Enum

        /// <summary>
        /// Obtém texto de exibição automático para enum
        /// </summary>
        public static string GetEnumDisplayText(Enum enumValue)
        {
            // 1. Tentar DisplayAttribute
            var displayAttr = GetEnumAttribute<DisplayAttribute>(enumValue);
            if (!string.IsNullOrEmpty(displayAttr?.Name))
            {
                return displayAttr.Name;
            }

            // 2. Tentar DescriptionAttribute
            var description = GetEnumDescription(enumValue);
            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            // 3. Converter nome usando convenção
            return ConvertEnumNameToDisplayText(enumValue.ToString());
        }

        /// <summary>
        /// Obtém descrição do enum
        /// </summary>
        public static string GetEnumDescription(Enum enumValue)
        {
            var descAttr = GetEnumAttribute<DescriptionAttribute>(enumValue);
            return descAttr?.Description ?? "";
        }

        /// <summary>
        /// Obtém categoria do enum
        /// </summary>
        public static string GetEnumCategory(Enum enumValue)
        {
            var categoryAttr = GetEnumAttribute<CategoryAttribute>(enumValue);
            return categoryAttr?.Category ?? "";
        }

        /// <summary>
        /// Obtém ícone do enum
        /// </summary>
        public static string GetEnumIcon(Enum enumValue)
        {
            // Verificar se existe atributo customizado para ícone
            var iconAttr = GetEnumAttribute<IconAttribute>(enumValue);
            if (!string.IsNullOrEmpty(iconAttr?.Icon))
            {
                return iconAttr.Icon;
            }

            // Auto-detect ícone baseado no nome
            return AutoDetectEnumIcon(enumValue.ToString());
        }

        /// <summary>
        /// Obtém classe CSS do enum
        /// </summary>
        public static string GetEnumCssClass(Enum enumValue)
        {
            var cssAttr = GetEnumAttribute<CssClassAttribute>(enumValue);
            if (!string.IsNullOrEmpty(cssAttr?.CssClass))
            {
                return cssAttr.CssClass;
            }

            // Auto-detect CSS baseado no nome
            return AutoDetectEnumCssClass(enumValue.ToString());
        }

        /// <summary>
        /// Verifica se enum está ativo
        /// </summary>
        public static bool GetEnumIsActive(Enum enumValue)
        {
            var activeAttr = GetEnumAttribute<ActiveAttribute>(enumValue);
            return activeAttr?.IsActive ?? true; // Por padrão, considerar ativo
        }

        #endregion

        #region Auto-Generation de JavaScript

        /// <summary>
        /// Gera JavaScript automático para todos os enums
        /// </summary>
        public static string GenerateJavaScriptForAllEnums()
        {
            var js = new List<string>
            {
                "// Auto-generated Enum definitions",
                "window.AutoEnums = window.AutoEnums || {};"
            };

            foreach (var enumType in GetAllEnumTypes())
            {
                js.Add(GenerateJavaScriptForEnum(enumType));
            }

            return string.Join(Environment.NewLine, js);
        }

        /// <summary>
        /// Gera JavaScript para um enum específico
        /// </summary>
        public static string GenerateJavaScriptForEnum(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                return "";
            }

            var enumInfo = GetEnumInfo(enumType);
            var jsEnumName = enumType.Name;

            var jsValues = enumInfo.Values.Select(v => 
                $"    {v.Name}: {{ value: {v.Value}, text: '{v.DisplayText}', icon: '{v.Icon}', cssClass: '{v.CssClass}' }}");

            return $@"window.AutoEnums.{jsEnumName} = {{
{string.Join(",\n", jsValues)}
}};";
        }

        #endregion

        #region Convention-Based Helpers

        /// <summary>
        /// Converte nome do enum para texto de exibição usando convenções
        /// </summary>
        private static string ConvertEnumNameToDisplayText(string enumName)
        {
            // Converter CamelCase para palavras separadas
            var result = System.Text.RegularExpressions.Regex.Replace(
                enumName, 
                "([a-z])([A-Z])", 
                "$1 $2");

            // Capitalizar primeira letra
            return char.ToUpper(result[0]) + result[1..];
        }

        /// <summary>
        /// Auto-detecta ícone baseado no nome do enum
        /// </summary>
        private static string AutoDetectEnumIcon(string enumName)
        {
            var name = enumName.ToLower();
            
            return name switch
            {
                var n when n.Contains("ativo") || n.Contains("active") => "fas fa-check-circle",
                var n when n.Contains("inativo") || n.Contains("inactive") => "fas fa-times-circle",
                var n when n.Contains("pendente") || n.Contains("pending") => "fas fa-clock",
                var n when n.Contains("aprovado") || n.Contains("approved") => "fas fa-thumbs-up",
                var n when n.Contains("rejeitado") || n.Contains("rejected") => "fas fa-thumbs-down",
                var n when n.Contains("novo") || n.Contains("new") => "fas fa-plus-circle",
                var n when n.Contains("processando") || n.Contains("processing") => "fas fa-spinner",
                var n when n.Contains("completo") || n.Contains("complete") => "fas fa-check",
                var n when n.Contains("cancelado") || n.Contains("cancelled") => "fas fa-ban",
                var n when n.Contains("erro") || n.Contains("error") => "fas fa-exclamation-triangle",
                var n when n.Contains("sucesso") || n.Contains("success") => "fas fa-check-circle",
                var n when n.Contains("warning") || n.Contains("aviso") => "fas fa-exclamation-triangle",
                var n when n.Contains("info") || n.Contains("informacao") => "fas fa-info-circle",
                var n when n.Contains("vendido") || n.Contains("sold") => "fas fa-handshake",
                var n when n.Contains("disponivel") || n.Contains("available") => "fas fa-check",
                var n when n.Contains("reservado") || n.Contains("reserved") => "fas fa-bookmark",
                _ => "fas fa-circle"
            };
        }

        /// <summary>
        /// Auto-detecta classe CSS baseado no nome do enum
        /// </summary>
        private static string AutoDetectEnumCssClass(string enumName)
        {
            var name = enumName.ToLower();
            
            return name switch
            {
                var n when n.Contains("ativo") || n.Contains("aprovado") || n.Contains("sucesso") => "text-success",
                var n when n.Contains("inativo") || n.Contains("rejeitado") || n.Contains("erro") => "text-danger",
                var n when n.Contains("pendente") || n.Contains("processando") => "text-warning",
                var n when n.Contains("novo") || n.Contains("info") => "text-info",
                var n when n.Contains("cancelado") => "text-muted",
                _ => "text-primary"
            };
        }

        /// <summary>
        /// Obtém atributo específico de um enum value
        /// </summary>
        private static T? GetEnumAttribute<T>(Enum enumValue) where T : Attribute
        {
            var type = enumValue.GetType();
            var memberInfo = type.GetMember(enumValue.ToString());
            
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
                if (attributes.Length > 0)
                {
                    return (T)attributes[0];
                }
            }
            
            return null;
        }

        #endregion

        #region Cache Management

        /// <summary>
        /// Limpa todos os caches (útil para desenvolvimento)
        /// </summary>
        public static void ClearAllCaches()
        {
            _enumOptionsCache.Clear();
            _enumInfoCache.Clear();
        }

        /// <summary>
        /// Obtém estatísticas dos caches
        /// </summary>
        public static Dictionary<string, int> GetCacheStatistics()
        {
            return new Dictionary<string, int>
            {
                ["EnumOptionsCache"] = _enumOptionsCache.Count,
                ["EnumInfoCache"] = _enumInfoCache.Count,
                ["DiscoveredEnums"] = _allEnumTypes.Value.Count
            };
        }

        /// <summary>
        /// Pré-carrega todos os caches para performance
        /// </summary>
        public static async Task WarmUpCachesAsync()
        {
            await Task.Run(() =>
            {
                foreach (var enumType in GetAllEnumTypes())
                {
                    GetEnumOptions(enumType);
                    GetEnumInfo(enumType);
                }
            });
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Atributos customizados para enums
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IconAttribute(string icon) : Attribute
    {
        public string Icon { get; } = icon;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CssClassAttribute(string cssClass) : Attribute
    {
        public string CssClass { get; } = cssClass;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ActiveAttribute(bool isActive = true) : Attribute
    {
        public bool IsActive { get; } = isActive;
    }

    #endregion
}
