namespace FGT.Models.Report
{
    public class PreviewRequest
    {
        public string EntityType { get; set; } = string.Empty;
        public ReportTemplate Template { get; set; } = new();
    }
}
