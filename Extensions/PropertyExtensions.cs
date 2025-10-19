using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models.Grid;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AutoGestao.Extensions
{
    public static class PropertyExtensions
    {
        /// <summary>
        /// Obtém nome de exibição de uma propriedade
        /// </summary>
        public static string? GetDisplayName(this PropertyInfo property)
        {
            // 1. Tentar DisplayAttribute
            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            if (!string.IsNullOrEmpty(displayAttr?.Name))
            {
                return displayAttr.Name;
            }

            // 2. Tentar DisplayNameAttribute
            var displayNameAttr = property.GetCustomAttribute<DisplayNameAttribute>();
            if (!string.IsNullOrEmpty(displayNameAttr?.DisplayName))
            {
                return displayNameAttr.DisplayName;
            }

            // 3. Tentar FormFieldAttribute
            var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();
            if (!string.IsNullOrEmpty(formFieldAttr?.Name))
            {
                return formFieldAttr.Name;
            }

            // 4. Converter nome da propriedade
            return ConvertToDisplayName(property.Name);
        }

        public static int GetGridOrder(this PropertyInfo property)
        {
            var gridAttr = property.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name.StartsWith("Grid"));

            // Usar reflexão para obter Order se existir
            var orderProp = gridAttr?.GetType().GetProperty("Order");
            return orderProp?.GetValue(gridAttr) as int? ?? 999;
        }

        public static object? GetGridAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttributes()
                .FirstOrDefault(attr => attr.GetType().Name.StartsWith("Grid"));
        }

        public static FormFieldAttribute? GetFormFieldAttribute(this PropertyInfo property)
        {
            return property.GetCustomAttribute<FormFieldAttribute>();
        }

        /// <summary>
        /// Converte nome de propriedade para display name
        /// </summary>
        public static string ConvertToDisplayName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return propertyName;
            }

            // Converter CamelCase para palavras separadas
            var result = System.Text.RegularExpressions.Regex.Replace(
                propertyName,
                "([a-z])([A-Z])",
                "$1 $2");

            // Capitalizar primeira letra
            return char.ToUpper(result[0]) + result[1..];
        }

        /// <summary>
        /// Obtém ícone padrão baseado no nome da propriedade
        /// </summary>
        public static string GetDefaultIcon(PropertyInfo property)
        {
            var name = property.Name.ToLower();
            var type = property.PropertyType;

            // Ícones baseados no nome
            return name switch
            {
                var n when n.Contains("email") => "fas fa-envelope",
                var n when n.Contains("telefone") || n.Contains("phone") => "fas fa-phone",
                var n when n.Contains("endereco") || n.Contains("address") => "fas fa-map-marker-alt",
                var n when n.Contains("cep") => "fas fa-mail-bulk",
                var n when n.Contains("cpf")  => "fas fa-fingerprint",
                var n when n.Contains("cnpj") => "fas fa-building",
                var n when n.Contains("data") || n.Contains("date") => "fas fa-calendar",
                var n when n.Contains("valor") || n.Contains("preco") || n.Contains("price") => "fas fa-dollar-sign",
                var n when n.Contains("codigo") || n.Contains("code") => "fas fa-barcode",
                var n when n.Contains("nome") || n.Contains("name") => "fas fa-signature",
                var n when n.Contains("descricao") || n.Contains("description") => "fas fa-align-left",
                var n when n.Contains("observ") => "fas fa-sticky-note",
                var n when n.Contains("status") || n.Contains("situacao") => "fas fa-flag",
                var n when n.Contains("ativo") || n.Contains("active") => "fas fa-toggle-on",
                var n when n.Contains("foto") || n.Contains("image") => "fas fa-image",
                var n when n.Contains("arquivo") || n.Contains("file") => "fas fa-file",
                _ => GetIconByType(type)
            };
        }

        /// <summary>
        /// Obtém ícone baseado no tipo da propriedade
        /// </summary>
        private static string GetIconByType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType switch
            {
                var t when t == typeof(bool) => "fas fa-check-square",
                var t when t == typeof(DateTime) => "fas fa-calendar",
                var t when t == typeof(decimal) || t == typeof(double) || t == typeof(float) => "fas fa-calculator",
                var t when t == typeof(int) || t == typeof(long) => "fas fa-hashtag",
                var t when t.IsEnum => "fas fa-list",
                var t when t == typeof(string) => "fas fa-font",
                _ => "fas fa-edit"
            };
        }

        /// <summary>
        /// Obtém placeholder padrão baseado na propriedade
        /// </summary>
        public static string GetDefaultPlaceholder(PropertyInfo property)
        {
            var displayName = GetDisplayName(property);
            var name = property.Name.ToLower();

            return name switch
            {
                var n when n.Contains("email") => "exemplo@email.com",
                var n when n.Contains("telefone") => "(11) 99999-9999",
                var n when n.Contains("cep") => "00000-000",
                var n when n.Contains("cpf") => "000.000.000-00",
                var n when n.Contains("cnpj") => "00.000.000/0000-00",
                var n when n.Contains("url") => "https://exemplo.com",
                var n when n.Contains("codigo") => "Ex: COD001",
                var n when n.Contains("observ") => "Digite suas observações...",
                var n when n.Contains("descricao") => "Descreva aqui...",
                _ => $"Digite {displayName.ToLower()}..."
            };
        }

        public static string GetDefaultPlaceholderByEnum(PropertyInfo property)
        {
            var propertyName = property.Name.ToLower();
            var fieldType = DetermineFieldType(property);

            return fieldType switch
            {
                EnumFieldType.Email => "exemplo@email.com",
                EnumFieldType.Telefone => "(00) 00000-0000",
                EnumFieldType.Cpf => "000.000.000-00",
                EnumFieldType.Cnpj => "00.000.000/0000-00",
                EnumFieldType.Cep => "00000-000",
                EnumFieldType.Currency => "R$ 0,00",
                EnumFieldType.Date => "dd/mm/aaaa",
                EnumFieldType.TextArea => $"Digite as {GetDisplayName(property).ToLower()}...",
                _ => $"Digite {GetDisplayName(property).ToLower()}"
            };
        }

        /// <summary>
        /// Obtém ícone para seção do formulário
        /// </summary>
        public static string GetSectionIcon(string sectionName)
        {
            var name = sectionName.ToLower();

            return name switch
            {
                var n when n.Contains("identificacao") || n.Contains("dados") => "fas fa-id-card",
                var n when n.Contains("endereco") || n.Contains("localizacao") => "fas fa-map-marker-alt",
                var n when n.Contains("contato") => "fas fa-phone",
                var n when n.Contains("financeiro") || n.Contains("pagamento") => "fas fa-dollar-sign",
                var n when n.Contains("documento") => "fas fa-file-alt",
                var n when n.Contains("status") || n.Contains("situacao") => "fas fa-flag",
                var n when n.Contains("observ") || n.Contains("adicional") => "fas fa-sticky-note",
                var n when n.Contains("configuracao") => "fas fa-cog",
                var n when n.Contains("auditoria") => "fas fa-history",
                _ => "fas fa-edit"
            };
        }

        /// <summary>
        /// Obtém ordem da seção
        /// </summary>
        public static int GetSectionOrder(string sectionName)
        {
            var name = sectionName.ToLower();

            return name switch
            {
                var n when n.Contains("identificacao") || n.Contains("principal") => 1,
                var n when n.Contains("dados") => 2,
                var n when n.Contains("endereco") => 3,
                var n when n.Contains("contato") => 4,
                var n when n.Contains("financeiro") => 5,
                var n when n.Contains("status") => 6,
                var n when n.Contains("observ") => 7,
                var n when n.Contains("adicional") => 8,
                var n when n.Contains("auditoria") => 9,
                _ => 5
            };
        }

        private static EnumFieldType DetermineFieldType(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (type.IsEnum)
            {
                return EnumFieldType.Select;
            }

            return type switch
            {
                Type t when t == typeof(string) => EnumFieldType.Text,
                Type t when t == typeof(int) || t == typeof(long) => EnumFieldType.Number,
                Type t when t == typeof(decimal) || t == typeof(double) || t == typeof(float) => EnumFieldType.Currency,
                Type t when t == typeof(DateTime) => EnumFieldType.Date,
                Type t when t == typeof(bool) => EnumFieldType.Checkbox,
                _ => EnumFieldType.Text
            };
        }
    }
}
