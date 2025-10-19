namespace AutoGestao.Helpers
{
    /// <summary>
    /// Informações sobre um valor de Enum
    /// </summary>
    public class EnumValueInfo
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
        public string DisplayText { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public string Icon { get; set; } = "";
        public string CssClass { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
