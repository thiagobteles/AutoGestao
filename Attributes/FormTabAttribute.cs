namespace AutoGestao.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FormTabAttribute(string tabId, string tabName) : Attribute
    {
        public string TabId { get; set; } = tabId;
        public string TabName { get; set; } = tabName;
        public string TabIcon { get; set; } = "fas fa-edit";
        public int Order { get; set; } = 0;
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public bool LazyLoad { get; set; } = true;
        public string[] RequiredRoles { get; set; } = [];
    }
}
