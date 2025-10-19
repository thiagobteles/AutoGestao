namespace AutoGestao.Models.Grid
{
    public class GridCellViewModel
    {
        public object Item { get; set; } = new();
        public GridColumn Column { get; set; } = new();
        public List<GridRowAction>? Actions { get; set; }
    }
}
