using AutoGestao.Entidades;

namespace AutoGestao.Models
{
    public class BaseViewModel<T> where T : BaseEntidade
    {
        public List<T> ListaObjeto { get; set; } = [];

        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }

        // Filtros
        public string? Search { get; set; }

        // Ordenação
        public string? OrderBy { get; set; } = "Marca";
        public string? OrderDirection { get; set; } = "asc";

        // Propriedades calculadas
        public bool HasFilters => Search != null && !string.IsNullOrEmpty(Search);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartRecord => PageSize == -1 ? 1 : (CurrentPage - 1) * PageSize + 1;
        public int EndRecord => PageSize == -1 ? TotalRecords : Math.Min(CurrentPage * PageSize, TotalRecords);

        public List<int> PageSizeOptions => [50, 100, 200, -1]; // -1 representa "Todos"

        public string GetPageSizeText(int size) => size == -1 ? "Todos" : size.ToString();
    }
}