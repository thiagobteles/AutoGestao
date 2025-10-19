namespace AutoGestao.Models.Grid
{
    public class GridRowAction
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public string CssClass { get; set; } = "btn btn-sm btn-outline-primary";
        public string? Url { get; set; }
        public string? OnClick { get; set; }
        public bool RequiresSelection { get; set; } = false;
        public bool RequireConfirmation { get; set; } = false;
        public string ConfirmationMessage { get; set; } = "Tem certeza?";
        public Func<object, bool>? ShowCondition { get; set; }
        public string? Target { get; set; } // _blank, _self, etc.
        public Dictionary<string, string> DataAttributes { get; set; } = [];
        public int Order { get; set; } = 0;
        public bool IsAjax { get; set; } = false;
        public string HttpMethod { get; set; } = "GET";
    }
}
