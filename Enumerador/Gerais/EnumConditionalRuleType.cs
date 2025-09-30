namespace AutoGestao.Enumerador.Gerais
{
    /// <summary>
    /// Tipos de regras condicionais
    /// </summary>
    public enum EnumConditionalRuleType
    {
        /// <summary>
        /// Controla visibilidade do campo
        /// </summary>
        Display,

        /// <summary>
        /// Controla se o campo é obrigatório
        /// </summary>
        Required,

        /// <summary>
        /// Controla se o campo é somente leitura
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Controla se o campo está habilitado
        /// </summary>
        Enabled,

        /// <summary>
        /// Validação customizada
        /// </summary>
        Validation
    }
}
