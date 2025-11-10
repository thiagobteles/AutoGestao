namespace FGT.Models.Report
{
    public class SaveTemplateRequest
    {
        public long? TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public ReportTemplate Template { get; set; } = new();
    }
}
