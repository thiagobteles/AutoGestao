using AutoGestao.Atributes;
using AutoGestao.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Helper para automação de propriedades Enum nas telas Create/Edit/Details
    /// </summary>
    public static class EnumAutomationHelper
    {
        /// <summary>
        /// Obtém todas as propriedades Enum de uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <returns>Lista de propriedades que são Enum</returns>
        public static List<PropertyInfo> GetEnumProperties<T>()
        {
            return [.. typeof(T).GetProperties()
                .Where(p => {
                    var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    return type.IsEnum;
                })];
        }

        /// <summary>
        /// Obtém automaticamente as opções de SelectList para uma propriedade Enum
        /// </summary>
        /// <param name="propertyName">Nome da propriedade</param>
        /// <param name="entityType">Tipo da entidade</param>
        /// <param name="includeIcons">Se deve incluir ícones (padrão: true)</param>
        /// <returns>Lista de SelectListItem ou lista vazia se não for Enum</returns>
        public static List<SelectListItem> GetEnumSelectListItems(string propertyName, Type entityType, bool includeIcons = true)
        {
            try
            {
                var property = entityType.GetProperty(propertyName);
                if (property == null)
                {
                    return [];
                }

                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (!propertyType.IsEnum)
                {
                    return [];
                }

                return GetEnumSelectListItems(propertyType, includeIcons);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter SelectListItems para {propertyName}: {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Obtém SelectListItems para um tipo Enum específico
        /// </summary>
        /// <param name="enumType">Tipo do Enum</param>
        /// <param name="includeIcons">Se deve incluir ícones</param>
        /// <returns>Lista de SelectListItem</returns>
        public static List<SelectListItem> GetEnumSelectListItems(Type enumType, bool includeIcons = true)
        {
            try
            {
                if (!enumType.IsEnum)
                {
                    return [];
                }

                var enumExtensionMethod = typeof(EnumExtension).GetMethod("GetSelectListItems");
                var genericMethod = enumExtensionMethod?.MakeGenericMethod(enumType);

                var result = (List<SelectListItem>?)genericMethod?.Invoke(null, [includeIcons]);

                return result ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter SelectListItems para enum {enumType.Name}: {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Popula automaticamente a ViewBag com todos os Enums de uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="viewBag">ViewBag do controller</param>
        /// <param name="includeIcons">Se deve incluir ícones</param>
        public static void PopulateEnumsInViewBag<T>(dynamic viewBag, bool includeIcons = true)
        {
            var enumProperties = GetEnumProperties<T>();

            foreach (var property in enumProperties)
            {
                var options = GetEnumSelectListItems(property.Name, typeof(T), includeIcons);
                if (options.Count != 0)
                {
                    // Adiciona na ViewBag usando o nome da propriedade
                    ((IDictionary<string, object>)viewBag)[property.Name] = options;
                }
            }
        }

        /// <summary>
        /// Popula automaticamente o ViewData com todos os Enums de uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="viewData">ViewData do controller</param>
        /// <param name="includeIcons">Se deve incluir ícones</param>
        public static void PopulateEnumsInViewData<T>(IDictionary<string, object> viewData, bool includeIcons = true)
        {
            var enumProperties = GetEnumProperties<T>();

            foreach (var property in enumProperties)
            {
                var options = GetEnumSelectListItems(property.Name, typeof(T), includeIcons);
                if (options.Count != 0)
                {
                    viewData[property.Name] = options;
                }
            }
        }

        /// <summary>
        /// Verifica se uma propriedade é um Enum
        /// </summary>
        /// <param name="propertyName">Nome da propriedade</param>
        /// <param name="entityType">Tipo da entidade</param>
        /// <returns>True se for Enum</returns>
        public static bool IsEnumProperty(string propertyName, Type entityType)
        {
            var property = entityType.GetProperty(propertyName);
            if (property == null)
            {
                return false;
            }

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return type.IsEnum;
        }

        /// <summary>
        /// Obtém informações detalhadas sobre um Enum
        /// </summary>
        /// <param name="enumType">Tipo do Enum</param>
        /// <returns>Informações do Enum</returns>
        public static EnumInfo GetEnumInfo(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                return new EnumInfo { Name = enumType.Name, IsEnum = false };
            }

            var values = Enum.GetValues(enumType);
            var enumInfo = new EnumInfo
            {
                Name = enumType.Name,
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
                    Description = enumValue.GetDescription(),
                    Category = enumValue.GetCategory(),
                    Icon = enumValue.GetIcone()
                });
            }

            return enumInfo;
        }

        /// <summary>
        /// Obtém um dicionário com todos os Enums de uma entidade e suas opções
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="includeIcons">Se deve incluir ícones</param>
        /// <returns>Dicionário com propriedade => opções</returns>
        public static Dictionary<string, List<SelectListItem>> GetAllEnumOptions<T>(bool includeIcons = true)
        {
            var result = new Dictionary<string, List<SelectListItem>>();
            var enumProperties = GetEnumProperties<T>();

            foreach (var property in enumProperties)
            {
                var options = GetEnumSelectListItems(property.Name, typeof(T), includeIcons);
                if (options.Count != 0)
                {
                    result[property.Name] = options;
                }
            }

            return result;
        }

        /// <summary>
        /// Gera código JavaScript com os Enums para uso no frontend
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <returns>String com código JavaScript</returns>
        public static string GenerateJavaScriptEnums<T>()
        {
            var enumProperties = GetEnumProperties<T>();
            var jsCode = new List<string> { "// Auto-generated Enum definitions", "window.Enums = window.Enums || {};" };

            foreach (var property in enumProperties)
            {
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var enumInfo = GetEnumInfo(propertyType);

                var jsEnum = $"window.Enums.{property.Name} = {{";
                var enumValues = enumInfo.Values.Select(v => $"  {v.Name}: {{ value: {v.Value}, description: '{v.Description}', icon: '{v.Icon}' }}");
                jsEnum += string.Join(",\n", enumValues);
                jsEnum += "\n};";

                jsCode.Add(jsEnum);
            }

            return string.Join("\n\n", jsCode);
        }

        /// <summary>
        /// Obtém informações de um valor específico de enum
        /// </summary>
        /// <param name="enumValue">Valor do enum</param>
        /// <returns>Informações do valor do enum</returns>
        public static EnumValueInfo GetEnumValueInfo(object enumValue)
        {
            if (enumValue == null)
            {
                return new EnumValueInfo
                {
                    Name = "",
                    Description = "",
                    Icon = "",
                    Value = 0
                };
            }

            var enumType = enumValue.GetType();
            if (!enumType.IsEnum)
            {
                return new EnumValueInfo
                {
                    Name = enumValue.ToString() ?? "",
                    Description = enumValue.ToString() ?? "",
                    Icon = "",
                    Value = 0
                };
            }

            var enumAsEnum = (Enum)enumValue;

            return new EnumValueInfo
            {
                Name = enumValue.ToString() ?? "",
                Value = Convert.ToInt32(enumValue),
                Description = enumAsEnum.GetDescription(),
                Category = enumAsEnum.GetCategory(),
                Icon = enumAsEnum.GetIcone() ?? "fas fa-circle"
            };
        }

        /// <summary>
        /// Obtém a cor de um valor de enum (baseado em atributos ou padrão)
        /// </summary>
        /// <param name="enumValue">Valor do enum</param>
        /// <returns>Cor em formato CSS</returns>
        public static string GetEnumColor(object enumValue)
        {
            if (enumValue == null)
            {
                return "#6c757d";
            }

            var enumType = enumValue.GetType();
            if (!enumType.IsEnum)
            {
                return "#6c757d";
            }

            var fieldInfo = enumType.GetField(enumValue.ToString()!);
            if (fieldInfo == null)
            {
                return "#6c757d";
            }

            // Buscar atributo de cor se existir
            var corAttr = fieldInfo.GetCustomAttributes(typeof(CorAttribute), false)
                .FirstOrDefault() as CorAttribute;

            if (corAttr != null)
            {
                return corAttr.Cor;
            }

            // Cores padrão baseadas no nome do enum
            var enumName = enumValue.ToString()?.ToLower() ?? "";

            if (enumName.Contains("ativo") || enumName.Contains("aprovado") ||
                enumName.Contains("pago") || enumName.Contains("concluido"))
            {
                return "#198754"; // Verde
            }

            if (enumName.Contains("inativo") || enumName.Contains("cancelado") ||
                enumName.Contains("reprovado"))
            {
                return "#dc3545"; // Vermelho
            }

            if (enumName.Contains("pendente") || enumName.Contains("aguardando"))
            {
                return "#ffc107"; // Amarelo
            }

            if (enumName.Contains("andamento") || enumName.Contains("processo"))
            {
                return "#0dcaf0"; // Azul claro
            }

            return "#6c757d"; // Cinza padrão
        }
    }
}