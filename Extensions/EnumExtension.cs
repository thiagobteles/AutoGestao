using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Extensions
{
    public static class EnumExtension
    {
        public static Dictionary<int, string> GetEnumDictionary<T>()
        {
            return !typeof(T).IsEnum
                ? throw new ArgumentException("T não é do tipo Enum")
                : Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(t => (int)(object)t, t => t.GetDescription());
        }

        public static string GetDescription<T>(this T source)
        {
            var field = source.GetType().GetField(source?.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : source.ToString();
        }

        public static string GetCategory<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            var attributes = (CategoryAttribute[])field.GetCustomAttributes(typeof(CategoryAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Category : source.ToString();
        }

        public static string GetIcone<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            var attributes = (IconeAttribute[])field.GetCustomAttributes(typeof(IconeAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Icone : source.ToString();
        }

        public static string GetValueString<T>(this T source)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T não é do tipo Enum");
            }

            var field = source.GetType().GetField(source?.ToString());
            return field.GetValue(source).ToString();
        }

        public static List<SelectListItem> GetSelectListItems<TEnum>(bool obterIcone = false) where TEnum : struct, Enum
        {
            var options = new List<SelectListItem>();
            var enumDictionary = GetEnumDictionary<TEnum>();

            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), item.Key);

                if (obterIcone && !string.IsNullOrEmpty(enumValue.GetIcone()))
                {
                    options.Add(new SelectListItem
                    {
                        Value = item.Key.ToString(),
                        Text = $"{enumValue.GetIcone()} {enumValue.GetDescription()}".Trim()
                    });
                }
                else
                {
                    options.Add(new SelectListItem
                    {
                        Value = item.Key.ToString(),
                        Text = enumValue.GetDescription().Trim()
                    });
                }
                
            }
            return options;
        }

        /// <summary>
        /// Resolve o valor condicional para comparação
        /// Suporta: "PessoaFisica", "1", "EnumTipoPessoa.PessoaFisica"
        /// </summary>
        /// <param name="conditionalField">Nome do campo que contém o enum</param>
        /// <param name="conditionalValue">Valor a ser comparado</param>
        /// <param name="entityType">Tipo da entidade</param>
        /// <returns>Valor resolvido para comparação (geralmente o valor numérico do enum)</returns>
        public static string ResolveConditionalValue(string conditionalField, string conditionalValue, Type entityType)
        {
            if (string.IsNullOrEmpty(conditionalField) || string.IsNullOrEmpty(conditionalValue))
            {
                return conditionalValue;
            }

            // Se já é um número, retorna como está
            if (int.TryParse(conditionalValue, out _))
            {
                return conditionalValue;
            }

            // Casos especiais que não são enums
            if (conditionalValue.StartsWith(">") || conditionalValue.StartsWith("<") || conditionalValue.StartsWith("!"))
            {
                return conditionalValue;
            }

            try
            {
                // Busca a propriedade na entidade
                var property = entityType.GetProperty(conditionalField);
                if (property == null)
                {
                    return conditionalValue;
                }

                // Verifica se é um enum
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (!propertyType.IsEnum)
                {
                    return conditionalValue;
                }

                // Remove prefixo do enum se existir (ex: "EnumTipoPessoa.PessoaFisica" -> "PessoaFisica")
                var cleanValue = conditionalValue;
                if (conditionalValue.Contains('.'))
                {
                    cleanValue = conditionalValue.Split('.').Last();
                }

                // Tenta fazer parse do enum
                if (Enum.TryParse(propertyType, cleanValue, true, out var enumValue))
                {
                    // Retorna o valor numérico do enum
                    return ((int)enumValue).ToString();
                }

                // Se não conseguiu fazer parse, retorna o valor original
                return conditionalValue;
            }
            catch
            {
                // Em caso de erro, retorna o valor original
                return conditionalValue;
            }
        }

        /// <summary>
        /// Resolve múltiplos valores condicionais (separados por vírgula)
        /// </summary>
        public static string ResolveMultipleConditionalValues(string conditionalField, string conditionalValues, Type entityType)
        {
            if (string.IsNullOrEmpty(conditionalValues))
            {
                return conditionalValues;
            }

            // Se contém vírgula, processa múltiplos valores
            if (conditionalValues.Contains(','))
            {
                var values = conditionalValues.Split(',')
                    .Select(v => v.Trim())
                    .Select(v => ResolveConditionalValue(conditionalField, v, entityType));

                return string.Join(",", values);
            }

            return ResolveConditionalValue(conditionalField, conditionalValues, entityType);
        }

        /// <summary>
        /// Verifica se o valor atual do campo satisfaz a condição
        /// </summary>
        public static bool EvaluateCondition(string currentValue, string conditionalValue)
        {
            if (string.IsNullOrEmpty(conditionalValue))
            {
                return true;
            }

            // Operadores especiais
            if (conditionalValue == ">0")
            {
                return !string.IsNullOrEmpty(currentValue) &&
                       currentValue != "0" &&
                       currentValue != "false";
            }

            if (conditionalValue.StartsWith("!"))
            {
                var notValue = conditionalValue.Substring(1);
                return currentValue != notValue;
            }

            // Múltiplos valores (OR logic)
            if (conditionalValue.Contains(','))
            {
                var values = conditionalValue.Split(',').Select(v => v.Trim());
                return values.Contains(currentValue);
            }

            // Comparação direta
            return currentValue == conditionalValue;
        }
    }
}