namespace AutoGestao.Models
{
    public class TabbedFormViewModel : StandardFormViewModel
    {
        public List<FormTabViewModel> Tabs { get; set; } = [];

        public string ActiveTab { get; set; } = "principal";

        public bool EnableTabs { get; set; } = false;

        public long EntityId { get; set; } = 0;
    }
}
