
using FGT.Models.Grid;

namespace FGT.Models
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
        public string Icon { get; set; } = "fas fa-table";
        public List<Dictionary<string, object>> Rows { get; set; } = [];
        public bool ShowCreateButton { get; set; } = true;
        public bool ShowEditButton { get; set; } = true;
        public bool ShowDeleteButton { get; set; } = true;
        public bool ShowDetailsButton { get; set; } = true;

    }
}