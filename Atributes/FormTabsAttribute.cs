namespace AutoGestao.Atributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FormTabsAttribute : Attribute
    {
        public bool EnableTabs { get; set; } = true;

        public string DefaultTab { get; set; } = "principal";
    }
}
