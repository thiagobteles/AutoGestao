using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AutoGestao.Models
{
    public class TabColumnDefinition
    {
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string Width { get; set; }
        public string Format { get; set; }
        public int Order { get; set; }

        public object GetValue(object item)
        {
            if (item == null)
            {
                return null;
            }

            try
            {
                var property = item.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    return null;
                }

                var value = property.GetValue(item);
                if (value == null)
                {
                    return string.Empty;
                }

                // Aplicar formatação se especificada
                if (!string.IsNullOrEmpty(Format))
                {
                    if (value is DateTime dateValue)
                    {
                        return dateValue.ToString(Format);
                    }
                    else if (value is decimal decimalValue)
                    {
                        return Format == "C"
                            ? decimalValue.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))
                            : decimalValue.ToString(Format);
                    }
                    else if (value is double doubleValue)
                    {
                        return doubleValue.ToString(Format);
                    }
                    else if (value is int || value is long)
                    {
                        return Convert.ToDecimal(value).ToString(Format);
                    }
                }

                // Para enums, retornar o nome amigável
                if (value is Enum enumValue)
                {
                    return GetEnumDisplayName(enumValue);
                }

                // Para booleanos, retornar ícones
                if (value is bool boolValue)
                {
                    return boolValue ? "✓" : "✗";
                }

                return value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter valor de {PropertyName}: {ex.Message}");
                return string.Empty;
            }
        }

        private static string GetEnumDisplayName(Enum enumValue)
        {
            var type = enumValue.GetType();
            var memberInfo = type.GetMember(enumValue.ToString());

            if (memberInfo.Length > 0)
            {
                var displayAttr = memberInfo[0].GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                {
                    return displayAttr.Name ?? enumValue.ToString();
                }
            }

            return enumValue.ToString();
        }
    }
}