namespace AutoGestao.Atributes
{
    /// <summary>
    /// Controla quando um campo deve ser obrigatório baseado em regras condicionais
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConditionalRequiredAttribute(string rule, string errorMessage) : Attribute
    {
        /// <summary>
        /// Regra para tornar o campo obrigatório
        /// Exemplos:
        /// - "TipoCliente == PessoaFisica"
        /// - "TipoCliente == PessoaFisica AND Age(DataNascimento) >= 18"
        /// - "HasValue(Email) OR HasValue(Telefone)"
        /// </summary>
        public string Rule { get; set; } = rule;

        /// <summary>
        /// Mensagem de erro quando o campo obrigatório não for preenchido
        /// </summary>
        public string ErrorMessage { get; set; } = errorMessage;
    }
}
