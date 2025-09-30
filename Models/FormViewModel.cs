namespace AutoGestao.Models
{
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
}
