using AutoGestao.Enumerador.Gerais;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoGestao.Models.Grid
{
    public class GridFilter
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public EnumGridFilterType Type { get; set; } = EnumGridFilterType.Text;
        public object? Value { get; set; }
        public List<SelectListItem>? Options { get; set; }
        public string? Placeholder { get; set; }
    }
}
