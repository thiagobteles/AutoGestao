using AutoGestao.Enumerador.Gerais;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoGestao.Models
{
    public class FormFieldViewModel
    {
        public string PropertyName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public string Placeholder { get; set; } = "";
        public EnumFieldType Type { get; set; } = EnumFieldType.Text;
        public bool Required { get; set; } = false;
        public bool ReadOnly { get; set; } = false;
        public object? Value { get; set; }
        public string? DisplayText { get; set; }
        public string ValidationRegex { get; set; } = "";
        public string ValidationMessage { get; set; } = "";
        public int GridColumns { get; set; } = 1;
        public string CssClass { get; set; } = "";
        public string DataList { get; set; } = "";
        public int Order { get; set; } = 0;
        public string Section { get; set; } = "Não Informado";
        public string ConditionalDisplayRule { get; set; } = "";
        public string ConditionalRequiredRule { get; set; } = "";
        public string ConditionalRequiredMessage { get; set; } = "";
        public bool ShouldDisplay { get; set; } = true;
        public bool IsConditionallyRequired { get; set; } = false;
        public string? ReferenceFilters { get; set; }
        public List<SelectListItem> Options { get; set; } = [];
        public Type? Reference { get; set; }
        public ReferenceFieldConfig ReferenceConfig { get; set; } = new();
    }
}