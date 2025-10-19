using System.Text.RegularExpressions;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Avalia expressões condicionais para exibição e validação de campos
    /// Suporta operadores lógicos (AND/OR) e funções especiais
    /// </summary>
    public class ConditionalExpressionEvaluator
    {
        /// <summary>
        /// Avalia uma expressão condicional
        /// </summary>
        /// <param name="expression">Expressão a ser avaliada</param>
        /// <param name="entity">Entidade contendo os valores</param>
        /// <param name="entityType">Tipo da entidade</param>
        /// <returns>Resultado da avaliação (true/false)</returns>
        public static bool Evaluate(string expression, object entity, Type entityType)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return true;
            }

            try
            {
                // Divide a expressão por operadores lógicos
                if (expression.Contains(" OR ", StringComparison.OrdinalIgnoreCase))
                {
                    return EvaluateOrExpression(expression, entity, entityType);
                }

                if (expression.Contains(" AND ", StringComparison.OrdinalIgnoreCase))
                {
                    return EvaluateAndExpression(expression, entity, entityType);
                }

                // Avalia expressão simples
                return EvaluateSimpleExpression(expression, entity, entityType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao avaliar expressão '{expression}': {ex.Message}");
                return false;
            }
        }

        ///// <summary>
        ///// Avalia uma regra condicional contra uma entidade
        ///// </summary>
        //public static bool Evaluate(string rule, object entity, Type entityType)
        //{
        //    if (string.IsNullOrEmpty(rule) || entity == null)
        //    {
        //        return true;
        //    }

        //    try
        //    {
        //        // Implementação simplificada para as regras mais comuns
        //        // Em produção, usar uma biblioteca como DynamicExpresso ou similar

        //        // Regras do tipo: "PropertyName == Value"
        //        if (rule.Contains("=="))
        //        {
        //            var parts = rule.Split("==", StringSplitOptions.TrimEntries);
        //            if (parts.Length == 2)
        //            {
        //                var propertyName = parts[0].Trim();
        //                var expectedValue = parts[1].Trim().Trim('"', '\'');

        //                var property = entityType.GetProperty(propertyName);
        //                if (property != null)
        //                {
        //                    var actualValue = property.GetValue(entity)?.ToString();
        //                    return string.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);
        //                }
        //            }
        //        }

        //        // Regras do tipo: "PropertyName != Value"
        //        if (rule.Contains("!="))
        //        {
        //            var parts = rule.Split("!=", StringSplitOptions.TrimEntries);
        //            if (parts.Length == 2)
        //            {
        //                var propertyName = parts[0].Trim();
        //                var expectedValue = parts[1].Trim().Trim('"', '\'');

        //                var property = entityType.GetProperty(propertyName);
        //                if (property != null)
        //                {
        //                    var actualValue = property.GetValue(entity)?.ToString();
        //                    return !string.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);
        //                }
        //            }
        //        }

        //        // Regras do tipo: "PropertyName > 0"
        //        if (rule.Contains(">"))
        //        {
        //            var parts = rule.Split(">", StringSplitOptions.TrimEntries);
        //            if (parts.Length == 2)
        //            {
        //                var propertyName = parts[0].Trim();
        //                var expectedValue = parts[1].Trim();

        //                var property = entityType.GetProperty(propertyName);
        //                if (property != null)
        //                {
        //                    var actualValue = property.GetValue(entity);
        //                    if (actualValue is IComparable comparable &&
        //                        decimal.TryParse(expectedValue, out var expectedDecimal))
        //                    {
        //                        var actualDecimal = Convert.ToDecimal(actualValue);
        //                        return actualDecimal > expectedDecimal;
        //                    }
        //                }
        //            }
        //        }

        //        // Regras simples de propriedade booleana: "IsActive"
        //        var boolProperty = entityType.GetProperty(rule.Trim());
        //        if (boolProperty?.PropertyType == typeof(bool))
        //        {
        //            return (bool)(boolProperty.GetValue(entity) ?? false);
        //        }

        //        // Se não conseguir avaliar, retornar true (mostrar por padrão)
        //        return true;
        //    }
        //    catch
        //    {
        //        // Em caso de erro, retornar true (mostrar por padrão)
        //        return true;
        //    }
        //}

        /// <summary>
        /// Avalia expressão com operador OR
        /// </summary>
        private static bool EvaluateOrExpression(string expression, object entity, Type entityType)
        {
            var parts = Regex.Split(expression, @"\s+OR\s+", RegexOptions.IgnoreCase);

            foreach (var part in parts)
            {
                if (Evaluate(part.Trim(), entity, entityType))
                {
                    return true; // Qualquer verdadeiro = resultado true
                }
            }

            return false;
        }

        /// <summary>
        /// Avalia expressão com operador AND
        /// </summary>
        private static bool EvaluateAndExpression(string expression, object entity, Type entityType)
        {
            var parts = Regex.Split(expression, @"\s+AND\s+", RegexOptions.IgnoreCase);

            foreach (var part in parts)
            {
                if (!Evaluate(part.Trim(), entity, entityType))
                {
                    return false; // Qualquer falso = resultado false
                }
            }

            return true;
        }

        /// <summary>
        /// Avalia uma expressão simples (sem operadores lógicos)
        /// </summary>
        private static bool EvaluateSimpleExpression(string expression, object entity, Type entityType)
        {
            expression = expression.Trim();

            // Verifica se é uma função
            if (expression.Contains("("))
            {
                return EvaluateFunction(expression, entity, entityType);
            }

            // Extrai partes da expressão: campo operador valor
            var match = Regex.Match(expression, @"^(\w+)\s*(==|!=|>|<|>=|<=)\s*(.+)$");

            if (!match.Success)
            {
                Console.WriteLine($"Expressão inválida: {expression}");
                return false;
            }

            var fieldName = match.Groups[1].Value.Trim();
            var operatorStr = match.Groups[2].Value.Trim();
            var expectedValue = match.Groups[3].Value.Trim();

            // Obtém o valor atual do campo
            var currentValue = GetFieldValue(fieldName, entity, entityType);

            // Avalia o operador
            return EvaluateOperator(currentValue, operatorStr, expectedValue, fieldName, entityType);
        }

        /// <summary>
        /// Avalia funções especiais
        /// </summary>
        private static bool EvaluateFunction(string expression, object entity, Type entityType)
        {
            // Age(campo) >= valor
            var ageMatch = Regex.Match(expression, @"Age\((\w+)\)\s*(==|!=|>|<|>=|<=)\s*(\d+)", RegexOptions.IgnoreCase);
            if (ageMatch.Success)
            {
                return EvaluateAgeFunction(ageMatch, entity, entityType);
            }

            // HasValue(campo)
            var hasValueMatch = Regex.Match(expression, @"HasValue\((\w+)\)", RegexOptions.IgnoreCase);
            if (hasValueMatch.Success)
            {
                return EvaluateHasValueFunction(hasValueMatch, entity, entityType);
            }

            // IsEmpty(campo)
            var isEmptyMatch = Regex.Match(expression, @"IsEmpty\((\w+)\)", RegexOptions.IgnoreCase);
            if (isEmptyMatch.Success)
            {
                return !EvaluateHasValueFunction(isEmptyMatch, entity, entityType);
            }

            // DateDiff(campo1, campo2, unidade) > valor
            var dateDiffMatch = Regex.Match(expression, @"DateDiff\((\w+),\s*(\w+),\s*(\w+)\)\s*(==|!=|>|<|>=|<=)\s*(\d+)", RegexOptions.IgnoreCase);
            if (dateDiffMatch.Success)
            {
                return EvaluateDateDiffFunction(dateDiffMatch, entity, entityType);
            }

            // Length(campo) > valor
            var lengthMatch = Regex.Match(expression, @"Length\((\w+)\)\s*(==|!=|>|<|>=|<=)\s*(\d+)", RegexOptions.IgnoreCase);
            if (lengthMatch.Success)
            {
                return EvaluateLengthFunction(lengthMatch, entity, entityType);
            }

            Console.WriteLine($"Função não reconhecida: {expression}");
            return false;
        }

        /// <summary>
        /// Avalia função Age() - calcula idade baseado em data de nascimento
        /// </summary>
        private static bool EvaluateAgeFunction(Match match, object entity, Type entityType)
        {
            var fieldName = match.Groups[1].Value;
            var operatorStr = match.Groups[2].Value;
            var expectedAge = int.Parse(match.Groups[3].Value);

            var dateValue = GetFieldValue(fieldName, entity, entityType);

            if (dateValue == null || !(dateValue is DateTime birthDate))
            {
                return false;
            }

            var age = CalculateAge(birthDate);
            return CompareNumbers(age, operatorStr, expectedAge);
        }

        /// <summary>
        /// Avalia função HasValue() - verifica se campo tem valor
        /// </summary>
        private static bool EvaluateHasValueFunction(Match match, object entity, Type entityType)
        {
            var fieldName = match.Groups[1].Value;
            var value = GetFieldValue(fieldName, entity, entityType);

            if (value == null)
            {
                return false;
            }

            if (value is string str)
            {
                return !string.IsNullOrWhiteSpace(str);
            }

            if (value is int intVal)
            {
                return intVal != 0;
            }

            if (value is decimal decVal)
            {
                return decVal != 0;
            }

            if (value is DateTime dateVal)
            {
                return dateVal != DateTime.MinValue;
            }

            return true;
        }

        /// <summary>
        /// Avalia função DateDiff() - diferença entre datas
        /// </summary>
        private static bool EvaluateDateDiffFunction(Match match, object entity, Type entityType)
        {
            var field1Name = match.Groups[1].Value;
            var field2Name = match.Groups[2].Value;
            var unit = match.Groups[3].Value.ToLower();
            var operatorStr = match.Groups[4].Value;
            var expectedDiff = int.Parse(match.Groups[5].Value);

            var date1 = GetFieldValue(field1Name, entity, entityType);
            var date2 = GetFieldValue(field2Name, entity, entityType);

            if (date1 == null || date2 == null || !(date1 is DateTime dt1) || !(date2 is DateTime dt2))
            {
                return false;
            }

            var diff = (dt1 - dt2).TotalDays;

            // Converte para a unidade desejada
            var actualDiff = unit switch
            {
                "days" => (int)diff,
                "months" => (int)(diff / 30),
                "years" => (int)(diff / 365),
                _ => (int)diff
            };

            return CompareNumbers(actualDiff, operatorStr, expectedDiff);
        }

        /// <summary>
        /// Avalia função Length() - tamanho de string
        /// </summary>
        private static bool EvaluateLengthFunction(Match match, object entity, Type entityType)
        {
            var fieldName = match.Groups[1].Value;
            var operatorStr = match.Groups[2].Value;
            var expectedLength = int.Parse(match.Groups[3].Value);

            var value = GetFieldValue(fieldName, entity, entityType);

            if (value == null)
            {
                return CompareNumbers(0, operatorStr, expectedLength);
            }

            var length = value.ToString()?.Length ?? 0;
            return CompareNumbers(length, operatorStr, expectedLength);
        }

        /// <summary>
        /// Obtém o valor de um campo da entidade
        /// </summary>
        private static object? GetFieldValue(string fieldName, object entity, Type entityType)
        {
            var property = entityType.GetProperty(fieldName);

            if (property == null)
            {
                return null;
            }

            return property.GetValue(entity);
        }

        /// <summary>
        /// Avalia um operador de comparação
        /// </summary>
        private static bool EvaluateOperator(object? currentValue, string operatorStr, string expectedValue, string fieldName, Type entityType)
        {
            // Resolve enum se necessário
            var resolvedExpectedValue = EnumConditionalHelper.ResolveConditionalValue(fieldName, expectedValue, entityType);

            // Converte valores para string para comparação
            var currentStr = ConvertToString(currentValue);
            var expectedStr = resolvedExpectedValue.Trim();

            return operatorStr switch
            {
                "==" => currentStr.Equals(expectedStr, StringComparison.OrdinalIgnoreCase),
                "!=" => !currentStr.Equals(expectedStr, StringComparison.OrdinalIgnoreCase),
                ">" => CompareNumeric(currentStr, expectedStr, (a, b) => a > b),
                "<" => CompareNumeric(currentStr, expectedStr, (a, b) => a < b),
                ">=" => CompareNumeric(currentStr, expectedStr, (a, b) => a >= b),
                "<=" => CompareNumeric(currentStr, expectedStr, (a, b) => a <= b),
                _ => false
            };
        }

        /// <summary>
        /// Compara valores numéricos
        /// </summary>
        private static bool CompareNumeric(string value1, string value2, Func<double, double, bool> comparison)
        {
            if (double.TryParse(value1, out var num1) && double.TryParse(value2, out var num2))
            {
                return comparison(num1, num2);
            }

            return false;
        }

        /// <summary>
        /// Compara números inteiros
        /// </summary>
        private static bool CompareNumbers(int value1, string operatorStr, int value2)
        {
            return operatorStr switch
            {
                "==" => value1 == value2,
                "!=" => value1 != value2,
                ">" => value1 > value2,
                "<" => value1 < value2,
                ">=" => value1 >= value2,
                "<=" => value1 <= value2,
                _ => false
            };
        }

        /// <summary>
        /// Converte um valor para string para comparação
        /// </summary>
        private static string ConvertToString(object? value)
        {
            if (value == null)
            {
                return "";
            }

            // Se for enum, retorna o valor numérico
            if (value.GetType().IsEnum)
            {
                return ((int)value).ToString();
            }

            return value.ToString() ?? "";
        }

        /// <summary>
        /// Calcula idade baseado em data de nascimento
        /// </summary>
        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        /// Gera JavaScript para avaliação no frontend
        /// </summary>
        public static string GenerateJavaScript(string expression, Type entityType)
        {
            // TODO: Implementar geração de JavaScript equivalente para validação client-side
            // Por enquanto, retorna a expressão original que será processada no JS
            return expression;
        }
    }
}