namespace AutoGestao.Models
{
    public class TabContentViewModel
    {
        public string EntityType { get; set; } = "";
        public string ControllerName { get; set; } = "";
        public string Mode { get; set; } = "Index";
        public long ParentId { get; set; }
        public List<object> Items { get; set; } = [];
        //public List<GridColumn> Columns { get; set; } = [];
        public List<TabColumnDefinition> Columns { get; set; } = [];
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "fas fa-list";
        public string? ForeignKeyProperty { get; set; }
        public bool CanCreate { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;
        public string TabId { get; set; }
        public string ParentController { get; set; }

        public TabContentViewModel()
        {
            Items = [];
            Columns = [];
        }
    }
}