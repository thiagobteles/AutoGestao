using FGT.Enumerador.Gerais;

namespace FGT.Models.Grid
{
    public class GridColumn
    {
        public string Name { get; set; } = "";
        public string Property { get; set; } = "";
        public string PropertyName { get; set; }
        public string DisplayName { get; set; } = "";
        public string? Width { get; set; }
        public bool Sortable { get; set; } = true;
        public bool Filterable { get; set; } = true;
        public string? CssClass { get; set; }
        public EnumGridColumnType Type { get; set; } = EnumGridColumnType.Text;
        public EnumRenderType? EnumRender { get; set; }
        public string? Format { get; set; }
        public string? UrlAction { get; set; }
        public Func<object, string>? CustomRender { get; set; }
        public int Order { get; set; }
        public string Template { get; set; }
        public string[] NavigationPaths { get; set; }
        public bool IsHtmlContent { get; set; }
    }
}
