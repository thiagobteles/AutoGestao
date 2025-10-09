using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Atributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReportFieldAttribute(string? label = null, string? section = null) : Attribute
    {
        public string? Label { get; set; } = label;
        public string? Section { get; set; } = section;
        public int Order { get; set; }
        public EnumReportFieldType Type { get; set; } = EnumReportFieldType.Text;
        public string? Format { get; set; }
        public bool ShowInSummary { get; set; } = false;
        public int GridColumns { get; set; } = 1;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReportTableAttribute(string title) : Attribute
    {
        public string Title { get; set; } = title;
        public bool ShowTotal { get; set; } = false;
        public string? TotalField { get; set; }
        public int Order { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReportConfigAttribute(string title) : Attribute
    {
        public string Title { get; set; } = title;
        public string? Subtitle { get; set; }
        public string? Icon { get; set; }
        public string? HeaderText { get; set; }
        public bool ShowLogo { get; set; } = true;
        public bool ShowDate { get; set; } = true;
        public bool ShowUser { get; set; } = true;
    }
}







