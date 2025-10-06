using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Atributes
{
    /* 
     * ============================================================
     * RESUMO DAS OPÇÕES SUPORTADAS PARA ConditionalValue:
     * ============================================================
     * 
     * 1. Nome do enum: "PessoaFisica" ✅
     * 2. Valor numérico: "1" ✅
     * 3. Nome completo: "EnumTipoPessoa.PessoaFisica" ✅
     * 4. Múltiplos valores: "1,2" (OR logic) ✅
     * 5. Maior que zero: ">0" ✅
     * 6. Diferente de: "!2" ✅
     * 
     * Todas essas opções funcionam automaticamente com a solução implementada!
     */

    /// <summary>
    /// Atributo para configuração de campos de formulário
    /// Define como uma propriedade deve ser renderizada nas telas Create/Edit/Details
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FormFieldAttribute : Attribute
    {
        /// <summary>
        /// Nome de exibição do campo
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Ícone do campo (classe CSS, ex: "fas fa-user")
        /// </summary>
        public string Icon { get; set; } = "fas fa-edit";

        /// <summary>
        /// Placeholder do campo
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Tipo do campo de formulário
        /// </summary>
        public EnumFieldType Type { get; set; }

        /// <summary>
        /// Indica se o campo é obrigatório
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// Indica se o campo é somente leitura
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Regex para validação customizada
        /// </summary>
        public string ValidationRegex { get; set; } = "";

        /// <summary>
        /// Mensagem de erro de validação
        /// </summary>
        public string ValidationMessage { get; set; } = "";

        /// <summary>
        /// Número de colunas do grid (1-6)
        /// </summary>
        public int GridColumns { get; set; } = 2;

        /// <summary>
        /// Classe CSS customizada
        /// </summary>
        public string CssClass { get; set; } = "";

        /// <summary>
        /// Data list para autocomplete
        /// </summary>
        public string DataList { get; set; } = "";

        /// <summary>
        /// Ordem de exibição do campo
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Seção do formulário onde o campo será exibido
        /// </summary>
        public string Section { get; set; } = "Gerais";

        /// <summary>
        /// Tipo da entidade de referência (para campos Reference)
        /// Exemplo: Reference = typeof(Cliente)
        /// </summary>
        public Type? Reference { get; set; }

        /// <summary>
        /// Configurações específicas para campo de referência
        /// </summary>
        public string ReferenceConfig { get; set; } = "";

        /// <summary>
        /// Tamanho da imagem em pixels (para Type = Image)
        /// Formato: "largura x altura" (ex: "150x150")
        /// Se não informado, usa 100x100
        /// </summary>
        public string ImageSize { get; set; } = "100x100";

        /// <summary>
        /// Extensões permitidas separadas por vírgula
        /// </summary>
        public string AllowedExtensions { get; set; } = "";

        /// <summary>
        /// Tamanho máximo do arquivo em MB
        /// </summary>
        public int MaxSizeMB { get; set; } = 10;
    }
}