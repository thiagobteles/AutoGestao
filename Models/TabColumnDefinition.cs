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
        public string[] NavigationPaths { get; set; }
        public string Template { get; set; }
        public bool IsHtmlContent { get; set; }

        public object GetValue(object item)
        {
            if (item == null)
            {
                return null;
            }

            try
            {
                // Se tem NavigationPaths definido, processar como campo composto
                if (NavigationPaths != null && NavigationPaths.Length > 0)
                {
                    var values = new List<object>();

                    foreach (var path in NavigationPaths)
                    {
                        var valueInterno = GetNestedPropertyValue(item, path);
                        values.Add(valueInterno ?? string.Empty);
                    }

                    // Se tem template, aplicar
                    if (!string.IsNullOrEmpty(Template))
                    {
                        try
                        {
                            return string.Format(Template, values.ToArray());
                        }
                        catch
                        {
                            // Se falhar o format, retornar valores separados
                            return string.Join(" - ", values.Where(v => v != null && !string.IsNullOrEmpty(v.ToString())));
                        }
                    }

                    // Sem template, retornar valores separados
                    return string.Join(" - ", values.Where(v => v != null && !string.IsNullOrEmpty(v.ToString())));
                }

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

        /// <summary>
        /// Obtém o valor de uma propriedade navegacional usando notação de ponto
        /// Exemplo: "EmpresaCliente.RazaoSocial" navegará por EmpresaCliente e depois pegará RazaoSocial
        /// </summary>
        private static object GetNestedPropertyValue(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            try
            {
                var parts = propertyPath.Split('.');
                object currentValue = obj;

                foreach (var part in parts)
                {
                    if (currentValue == null)
                    {
                        return null;
                    }

                    var property = currentValue.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
                    if (property == null)
                    {
                        return null;
                    }

                    currentValue = property.GetValue(currentValue);
                }

                return currentValue;
            }
            catch
            {
                return null;
            }
        }
    }
}