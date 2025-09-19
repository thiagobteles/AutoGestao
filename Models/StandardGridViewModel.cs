using Microsoft.AspNetCore.Mvc.Rendering;

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
        public string? OrderBy { get; set; }
        public string? OrderDirection { get; set; } = "asc";

        // Grid Configuration
        public string Title { get; set; } = "";
        public string SubTitle { get; set; } = "";
        public string EntityName { get; set; } = "";
        public string ControllerName { get; set; } = "";
        public List<GridColumn> Columns { get; set; } = [];
        public List<GridFilter> Filters { get; set; } = [];
        public List<GridAction> HeaderActions { get; set; } = [];
        public List<GridAction> RowActions { get; set; } = [];
    }

    public class GridColumn
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string? Width { get; set; }
        public bool Sortable { get; set; } = true;
        public string? CssClass { get; set; }
        public GridColumnType Type { get; set; } = GridColumnType.Text;
        public string? Format { get; set; }
        public Func<object, string>? CustomRender { get; set; }
    }

    public class GridFilter
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public GridFilterType Type { get; set; } = GridFilterType.Text;
        public object? Value { get; set; }
        public List<SelectListItem>? Options { get; set; }
        public string? Placeholder { get; set; }
    }

    public class GridAction
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Icon { get; set; } = "";
        public string CssClass { get; set; } = "";
        public string? Url { get; set; }
        public string? OnClick { get; set; }
        public bool RequiresSelection { get; set; } = false;
        public Func<object, bool>? ShowCondition { get; set; }
    }

    public class GridCellViewModel
    {
        public object Item { get; set; } = new();
        public GridColumn Column { get; set; } = new();
        public List<GridAction>? Actions { get; set; }
    }

    public enum GridColumnType
    {
        Text, Number, Currency, Date, Badge, Boolean, Custom, Actions
    }

    public enum GridFilterType
    {
        Text, Select, Date, DateRange, Number
    }
}