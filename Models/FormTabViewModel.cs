namespace FGT.Models
{
    public class FormTabViewModel
    {
        public string TabId { get; set; } = "";
        public string TabName { get; set; } = "";
        public string TabIcon { get; set; } = "fas fa-edit";
        public int Order { get; set; } = 0;
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public bool LazyLoad { get; set; } = true;
        public bool IsActive { get; set; } = false;
        public bool HasAccess { get; set; } = true;
        public string Content { get; set; } = "";
        public Dictionary<string, object> Parameters { get; set; } = [];
    }
}
