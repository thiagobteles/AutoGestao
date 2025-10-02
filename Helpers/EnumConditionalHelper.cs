namespace AutoGestao.Helpers
{
    public class EnumConditionalHelper
    {
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
                var notValue = conditionalValue[1..];
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
