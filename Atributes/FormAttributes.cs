using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Atributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormFieldAttribute : Attribute
    {
        public string DisplayName { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public string Placeholder { get; set; } = "";
        public FormFieldType Type { get; set; } = FormFieldType.Text;
        public bool Required { get; set; } = false;
        public bool ReadOnly { get; set; } = false;
        public int Order { get; set; } = 0;
        public string Section { get; set; } = "Dados Básicos";
        public string ValidationRegex { get; set; } = "";
        public string ValidationMessage { get; set; } = "";
        public string ConditionalField { get; set; } = "";
        public string ConditionalValue { get; set; } = "";
        public int GridColumns { get; set; } = 1; // 1, 2 ou 3 para grid-1, grid-2, grid-3
        public string CssClass { get; set; } = "";
        public string DataList { get; set; } = ""; // Para campos com autocomplete
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