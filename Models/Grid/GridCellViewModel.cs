namespace FGT.Models.Grid
{
    public class GridCellViewModel
    {
        public object Item { get; set; } = new();
        public GridColumn Column { get; set; } = new();
        public List<GridAction>? Actions { get; set; }
    }
}
