namespace FGT.Enumerador.Gerais
{
    public enum EnumFieldType
    {
        Text,
        Number,
        Decimal,
        Email,
        TextArea,
        Date,
        DateTime,
        Time,
        Checkbox,
        Percentage,

        // Campos com mask já criadas previamente
        Telefone,
        Cep,
        Cpf,
        Cnpj,
        Placa,
        Url,

        Hidden,
        Password, // Campo de senha
        Select, // Select dropdown (para Enums e listas)
        Currency, // Campo de moeda com máscara
        Reference, // Campo de referência com busca automática
        Image, // Campo de imagem com upload/download e preview minimalista
        File // Campo de arquivo com upload/download
    }
}