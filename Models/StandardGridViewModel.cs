using AutoGestao.Enumerador.Gerais;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoGestao.Models
{
    public class StandardGridViewModel
    {
        public StandardGridViewModel()
        {
        }

        public StandardGridViewModel(string titulo, string subTitulo, string nomeEntidade)
        {
            Title = titulo;
            SubTitle = subTitulo;
            EntityName = nomeEntidade;
            ControllerName = nomeEntidade;
            HeaderActions = ObterHeaderActionsPadrao(nomeEntidade);
            RowActions = ObterRowActionsPadrao(nomeEntidade);
        }

        public List<object> Items { get; set; } = [];
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages => PageSize == -1 ? 1 : (int)Math.Ceiling((double)TotalRecords / PageSize);
        public string? Search { get; set; }
        public string? OrderBy { get; set; } = "id";
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

        public List<GridAction> ObterHeaderActionsPadrao(string controllerNome)
        {
            return
                [
                    new()
                    {
                        Name = "Export",
                        DisplayName = "Exportar",
                        Icon = "fas fa-download",
                        CssClass = "btn-modern btn-outline-modern",
                        Url = "/" + controllerNome + "/Export"
                    },
                    new()
                    {
                        Name = "Create",
                        DisplayName = "Novo ",
                        Icon = "fas fa-plus",
                        CssClass = "btn-new",
                        Url = "/" + controllerNome + "/Create"
                    }
                ];
        }

        public List<GridAction> ObterRowActionsPadrao(string controllerNome)
        {
            return
                [
                    new()
                    {
                        Name = "Details",
                        DisplayName = "Visualizar",
                        Icon = "fas fa-eye",
                        Url = "/" + controllerNome + "/Details/{id}"
                    },
                    new()
                    {
                        Name = "Edit",
                        DisplayName = "Editar",
                        Icon = "fas fa-edit",
                        Url = "/" + controllerNome + "/Edit/{id}"
                    },
                    new()
                    {
                        Name = "Delete",
                        DisplayName = "Excluir",
                        Icon = "fas fa-trash",
                        Url = "/" + controllerNome + "/Delete/{id}"
                    },
                ];
        }
    }

    public class GridColumn
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string? Width { get; set; }
        public bool Sortable { get; set; } = true;
        public string? CssClass { get; set; }
        public EnumGridColumnType Type { get; set; } = EnumGridColumnType.Text;
        public EnumRenderType? EnumRender { get; set; }
        public string? Format { get; set; }
        public string? UrlAction { get; set; }
        public Func<object, string>? CustomRender { get; set; }
    }

    public class GridFilter
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public EnumGridFilterType Type { get; set; } = EnumGridFilterType.Text;
        public object? Value { get; set; }
        public List<SelectListItem>? Options {  get; set; }
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
}