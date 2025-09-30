namespace AutoGestao.Attributes
{
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
}
