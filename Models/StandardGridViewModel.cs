
using AutoGestao.Models.Grid;

namespace AutoGestao.Models
{
    public class StandardGridViewModel
    {
        public List<object> Items { get; set; } = [];
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages => PageSize == -1 ? 1 : (int)Math.Ceiling((double)TotalRecords / PageSize);
        public string? Search { get; set; }
        public string? OrderBy { get; set; } = "id";
        public string? OrderDirection { get; set; } = "asc";
        public string Title { get; set; } = "";
        public string SubTitle { get; set; } = "";
        public string EntityName { get; set; } = "";
        public string ControllerName { get; set; } = "";
        public List<GridColumn> Columns { get; set; } = [];
        public List<GridFilter> Filters { get; set; } = [];
        public List<GridAction> HeaderActions { get; set; } = [];
        public List<GridAction> RowActions { get; set; } = [];
    }
}