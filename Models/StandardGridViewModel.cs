
using AutoGestao.Models.Grid;

namespace AutoGestao.Models
{
    public class StandardGridViewModel<T> where T : class
    {
        public List<T> Items { get; set; } = [];
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
        public List<GridRowAction> HeaderActions { get; set; } = [];
        public List<GridRowAction> RowActions { get; set; } = [];
        public string Icon { get; set; } = "fas fa-table";
        public List<Dictionary<string, object>> Rows { get; set; } = [];
        public bool ShowCreateButton { get; set; } = true;
        public bool ShowEditButton { get; set; } = true;
        public bool ShowDeleteButton { get; set; } = true;
        public bool ShowDetailsButton { get; set; } = true;
        public string CreateUrl { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public Dictionary<string, object> AdditionalData { get; set; } = [];
    }

    public class StandardGridViewModel : StandardGridViewModel<object>
    {
        // Propriedade adicional para conversão
        public new List<object> Items
        {
            get => base.Items;
            set => base.Items = value;
        }
    }
}