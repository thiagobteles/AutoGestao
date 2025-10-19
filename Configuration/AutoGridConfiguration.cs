using AutoGestao.Models.Grid;

namespace AutoGestao.Configuration
{
    /// <summary>
    /// Configuração automática da grid
    /// </summary>
    public class AutoGridConfiguration
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "";
        public List<GridFilter> Filters { get; set; } = [];
        public List<GridColumn> Columns { get; set; } = [];
        public List<GridRowAction> RowActions { get; set; } = [];
    }
}
