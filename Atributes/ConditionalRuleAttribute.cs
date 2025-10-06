using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Atributes
{
    /// <summary>
    /// Define múltiplas regras condicionais para um campo
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConditionalRuleAttribute(EnumConditionalRuleType type, string expression) : Attribute
    {
        /// <summary>
        /// Nome da regra (para referência)
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Tipo de regra
        /// </summary>
        public EnumConditionalRuleType Type { get; set; } = type;

        /// <summary>
        /// Expressão da regra
        /// </summary>
        public string Expression { get; set; } = expression;

        /// <summary>
        /// Mensagem de erro (se aplicável)
        /// </summary>
        public string ErrorMessage { get; set; } = "";
    }
}
