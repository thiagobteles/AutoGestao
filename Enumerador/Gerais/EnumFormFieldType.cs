namespace AutoGestao.Enumerador.Gerais
{
    public enum EnumFieldType
    {
        /// <summary>
        /// Campo de texto simples
        /// </summary>
        Text,

        /// <summary>
        /// Campo numérico
        /// </summary>
        Number,

        /// <summary>
        /// Campo decimal
        /// </summary>
        Decimal,

        /// <summary>
        /// Campo de email com validação
        /// </summary>
        Email,

        /// <summary>
        /// Campo de senha
        /// </summary>
        Password,

        /// <summary>
        /// Área de texto (textarea)
        /// </summary>
        TextArea,

        /// <summary>
        /// Campo de data
        /// </summary>
        Date,

        /// <summary>
        /// Campo de data e hora
        /// </summary>
        DateTime,

        /// <summary>
        /// Campo de hora
        /// </summary>
        Time,

        /// <summary>
        /// Checkbox
        /// </summary>
        Checkbox,

        /// <summary>
        /// Select dropdown (para Enums e listas)
        /// </summary>
        Select,

        /// <summary>
        /// Campo de moeda com máscara
        /// </summary>
        Currency,

        /// <summary>
        /// Campo de porcentagem
        /// </summary>
        Percentage,

        /// <summary>
        /// Campo de telefone com máscara
        /// </summary>
        Phone,

        /// <summary>
        /// Campo de CEP com máscara
        /// </summary>
        Cep,

        /// <summary>
        /// Campo de CPF com máscara
        /// </summary>
        Cpf,

        /// <summary>
        /// Campo de CNPJ com máscara
        /// </summary>
        Cnpj,

        /// <summary>
        /// Campo oculto
        /// </summary>
        Hidden,

        /// <summary>
        /// Campo de referência com busca automática (NOVO)
        /// </summary>
        Reference
    }
}