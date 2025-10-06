namespace AutoGestao.Atributes
{
    /* 
     * ============================================================
     * RESUMO DAS FUNÇÕES SUPORTADAS:
     * ============================================================
     * 
     * ✅ Age(DataNascimento) >= 18
     *    Calcula idade baseado em data de nascimento
     * 
     * ✅ HasValue(campo)
     *    Verifica se campo tem valor preenchido
     * 
     * ✅ IsEmpty(campo)
     *    Verifica se campo está vazio
     * 
     * ✅ Length(campo) > 10
     *    Verifica tamanho de string
     * 
     * ✅ DateDiff(campo1, campo2, "days") > 30
     *    Diferença entre datas (days, months, years)
     * 
     * ============================================================
     * OPERADORES SUPORTADOS:
     * ============================================================
     * 
     * ==  Igual a
     * !=  Diferente de
     * >   Maior que
     * <   Menor que
     * >=  Maior ou igual
     * <=  Menor ou igual
     * AND Operador lógico E
     * OR  Operador lógico OU
     * 
     * ============================================================
     * EXEMPLOS DE EXPRESSÕES:
     * ============================================================
     * 
     * ✅ "TipoCliente == PessoaFisica"
     * ✅ "TipoCliente == PessoaFisica AND Age(DataNascimento) >= 18"
     * ✅ "Valor > 1000 OR Status == Aprovado"
     * ✅ "HasValue(Email) OR HasValue(Telefone)"
     * ✅ "Length(Nome) > 20 AND TipoCliente == PessoaFisica"
     * ✅ "DateDiff(DataVencimento, DataAtual, years) >= 1"
     */

    /// <summary>
    /// Controla quando um campo deve ser exibido baseado em regras condicionais
    /// Suporta múltiplas condições e operadores lógicos (AND/OR)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConditionalDisplayAttribute(string rule) : Attribute
    {
        /// <summary>
        /// Regra de exibição usando sintaxe de expressão
        /// Exemplos:
        /// - "TipoCliente == PessoaFisica"
        /// - "TipoCliente == PessoaFisica AND Age(DataNascimento) >= 10"
        /// - "TipoCliente == PessoaFisica OR TipoCliente == PessoaJuridica"
        /// - "Valor > 1000 AND Status == Aprovado"
        /// </summary>
        public string Rule { get; set; } = rule;
    }
}