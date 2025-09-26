using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Attributes
{
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
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// Ícone do campo (classe CSS, ex: "fas fa-user")
        /// </summary>
        public string Icon { get; set; } = "fas fa-edit";

        /// <summary>
        /// Placeholder do campo
        /// </summary>
        public string Placeholder { get; set; } = "";

        /// <summary>
        /// Tipo do campo de formulário
        /// </summary>
        public EnumFieldType Type { get; set; } = EnumFieldType.Text;

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
        /// Campo condicional (mostra/esconde baseado em outro campo)
        /// </summary>
        public string ConditionalField { get; set; } = "";

        /// <summary>
        /// Valor do campo condicional
        /// </summary>
        public string ConditionalValue { get; set; } = "";

        /// <summary>
        /// Número de colunas do grid (1-4)
        /// </summary>
        public int GridColumns { get; set; } = 1;

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
        public string Section { get; set; } = "Não Informado";

        /// <summary>
        /// NOVA PROPRIEDADE: Tipo da entidade de referência (para campos Reference)
        /// Exemplo: Reference = typeof(Cliente)
        /// </summary>
        public Type? Reference { get; set; }

        /// <summary>
        /// NOVA PROPRIEDADE: Configurações específicas para campo de referência
        /// </summary>
        public string ReferenceConfig { get; set; } = "";
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FormConfigAttribute : Attribute
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public bool EnableAjaxSubmit { get; set; } = true;
        public string BackAction { get; set; } = "Index";
        public string BackText { get; set; } = "Voltar à Lista";
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FormTabsAttribute : Attribute
    {
        public bool EnableTabs { get; set; } = true;
        public string DefaultTab { get; set; } = "principal";
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FormTabAttribute(string tabId, string tabName) : Attribute
    {
        public string TabId { get; set; } = tabId;
        public string TabName { get; set; } = tabName;
        public string TabIcon { get; set; } = "fas fa-edit";
        public int Order { get; set; } = 0;
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public bool LazyLoad { get; set; } = true;
        public string[] RequiredRoles { get; set; } = [];
    }
}