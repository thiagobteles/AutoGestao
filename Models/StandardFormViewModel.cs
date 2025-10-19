namespace AutoGestao.Models
{
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
        public string EntityName { get; set; } = "";
    }
}
