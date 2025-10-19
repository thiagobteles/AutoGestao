namespace AutoGestao.Models.Grid
{
    public class GridAction
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Icon { get; set; } = "";
        public string CssClass { get; set; } = "";
        public string? Url { get; set; }
        public string? OnClick { get; set; }
        public bool RequiresSelection { get; set; } = false;
        public Func<object, bool>? ShowCondition { get; set; }
    }
}
