namespace AutoGestao.Models
{
    public class TabbedFormViewModel : StandardFormViewModel
    {
        public bool EnableTabs { get; set; } = false;
        public long EntityId { get; set; } = 0;
        public string ActiveTab { get; set; } = "principal";
        public List<FormTabViewModel> Tabs { get; set; } = [];
    }
}
