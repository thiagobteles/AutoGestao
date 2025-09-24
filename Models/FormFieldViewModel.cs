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
        public EnumFormFieldType Type { get; set; } = EnumFormFieldType.Text;
        public bool Required { get; set; } = false;
        public bool ReadOnly { get; set; } = false;
        public object? Value { get; set; }
        public string ValidationRegex { get; set; } = "";
        public string ValidationMessage { get; set; } = "";
        public string ConditionalField { get; set; } = "";
        public string ConditionalValue { get; set; } = "";
        public int GridColumns { get; set; } = 1;
        public string CssClass { get; set; } = "";
        public string DataList { get; set; } = "";
        public int Order { get; set; } = 0;
        public string Section { get; set; } = "Dados Básicos";
        public List<SelectListItem> Options { get; set; } = [];
    }

    public class FormViewModel
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public string Action { get; set; } = "";
        public object Entity { get; set; } = new();
        public List<FormSectionViewModel> Sections { get; set; } = [];
        public List<FormTabViewModel> Tabs { get; set; } = [];
        public bool EnableAjaxSubmit { get; set; } = true;
        public string BackAction { get; set; } = "Index";
        public string BackText { get; set; } = "Voltar à Lista";
    }

    public class FormTabViewModel
    {
        public string TabId { get; set; } = "";
        public string TabName { get; set; } = "";
        public string TabIcon { get; set; } = "fas fa-edit";
        public int Order { get; set; } = 0;
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public bool LazyLoad { get; set; } = true;
        public bool IsActive { get; set; } = false;
        public bool HasAccess { get; set; } = true;
        public string Content { get; set; } = "";
        public Dictionary<string, object> Parameters { get; set; } = [];
    }

    public class StandardFormViewModel
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public string BackAction { get; set; } = "Index";
        public string BackText { get; set; } = "Voltar à Lista";
        public string ActionName { get; set; } = "";
        public string ControllerName { get; set; } = "";
        public object Model { get; set; }
        public List<FormSectionViewModel> Sections { get; set; } = [];
        public Dictionary<string, string> ModelState { get; set; } = [];
        public bool EnableAjaxSubmit { get; set; } = true;
        public bool IsEditMode { get; set; } = false;
        public bool IsDetailsMode { get; set; } = false;
    }

    public class FormSectionViewModel
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public int GridColumns { get; set; } = 1;
        public List<FormFieldViewModel> Fields { get; set; } = [];
    }

    public class TabbedFormViewModel : StandardFormViewModel
    {
        public List<FormTabViewModel> Tabs { get; set; } = [];
        public string ActiveTab { get; set; } = "principal";
        public bool EnableTabs { get; set; } = false;
        public int EntityId { get; set; } = 0;
    }
}