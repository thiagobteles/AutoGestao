using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Models.Report
{
    public class ReportTemplate
    {
        public string Name { get; set; } = string.Empty;
        public List<ReportSection> Sections { get; set; } = [];
    }

    public class ReportSection
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Type { get; set; } = "grid"; // grid ou table
        public int Columns { get; set; } = 3;
        public int Order { get; set; }
        public List<ReportField> Fields { get; set; } = [];
        public List<ReportColumn> ListColumns { get; set; } = [];
        public string? DataProperty { get; set; }
        public bool ShowTotal { get; set; }
        public string? TotalField { get; set; }
    }

    public class ReportField
    {
        public string Label { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string? Format { get; set; }
        public int Order { get; set; }
    }

    public class ReportColumn
    {
        public string Label { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string? Format { get; set; }
        public string Align { get; set; } = "left";
    }

    public class ReportFieldInfo
    {
        public string PropertyName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public EnumReportFieldType Type { get; set; }
        public int Order { get; set; }
    }
}
