using AutoGestao.Entidades;
using AutoGestao.Enumerador.Veiculo;

namespace AutoGestao.Models
{
    public class VeiculosIndexViewModel
    {
        public List<Veiculo> Veiculos { get; set; } = [];
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }

        // Filtros
        public string? Search { get; set; }
        public int? Situacao { get; set; }
        public int? Ano { get; set; }
        public string? Marca { get; set; }
        public string? Combustivel { get; set; }

        // Ordenação
        public string? OrderBy { get; set; } = "Marca";
        public string? OrderDirection { get; set; } = "asc";

        // Dados para filtros
        public List<EnumMarcaVeiculo> MarcasDisponiveis { get; set; } = [];
        public List<EnumCombustivelVeiculo> CombustiveisDisponiveis { get; set; } = [];
        public List<int> AnosDisponiveis { get; set; } = [];

        // Propriedades calculadas
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartRecord => PageSize == -1 ? 1 : (CurrentPage - 1) * PageSize + 1;
        public int EndRecord => PageSize == -1 ? TotalRecords : Math.Min(CurrentPage * PageSize, TotalRecords);

        public List<int> PageSizeOptions => [50, 100, 200, -1]; // -1 representa "Todos"

        public string GetPageSizeText(int size) => size == -1 ? "Todos" : size.ToString();
    }
}