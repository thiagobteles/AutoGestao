using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Models
{
    /// <summary>
    /// Request para busca de referências com suporte a filtros dinâmicos
    /// </summary>
    public class ReferenceSearchRequest
    {
        [Required(ErrorMessage = "EntityType é obrigatório")]
        public string EntityType { get; set; } = "";

        [Required(ErrorMessage = "SearchTerm é obrigatório")]
        [MinLength(2, ErrorMessage = "SearchTerm deve ter pelo menos 2 caracteres")]
        public string SearchTerm { get; set; } = "";

        [Range(1, 50, ErrorMessage = "PageSize deve estar entre 1 e 50")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Filtros dinâmicos a serem aplicados na busca
        /// Exemplo: { "IdVeiculoMarca": "5", "Ativo": "true" }
        /// </summary>
        public Dictionary<string, string> Filters { get; set; } = [];
    }

    /// <summary>
    /// Request para obter item por ID
    /// </summary>
    public class ReferenceGetByIdRequest
    {
        [Required(ErrorMessage = "EntityType é obrigatório")]
        public string EntityType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Id é obrigatório")]
        public string Id { get; set; } = string.Empty;
    }

    /// <summary>
    /// Item de referência retornado pela API
    /// </summary>
    public class ReferenceItem
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public Dictionary<string, object>? Extra { get; set; }
    }

    /// <summary>
    /// Configuração de filtros de um campo Reference
    /// </summary>
    public class ReferenceFilterConfig
    {
        /// <summary>
        /// Lista de filtros configurados via [ReferenceFilter]
        /// </summary>
        public List<ReferenceFilterInfo> Filters { get; set; } = [];

        /// <summary>
        /// Nome da propriedade que possui os filtros
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Informações de um filtro individual
    /// </summary>
    public class ReferenceFilterInfo
    {
        /// <summary>
        /// Campo a ser filtrado na entidade de referência
        /// </summary>
        public string FilterField { get; set; } = string.Empty;

        /// <summary>
        /// Valor do filtro (pode ser nome de propriedade ou valor literal)
        /// </summary>
        public string FilterValue { get; set; } = string.Empty;

        /// <summary>
        /// Operador de comparação
        /// </summary>
        public EnumFilterOperator Operator { get; set; } = EnumFilterOperator.Equals;

        /// <summary>
        /// Se true, FilterValue é nome de propriedade da entidade
        /// Se false, é um valor literal
        /// </summary>
        public bool IsPropertyReference { get; set; } = true;
    }
}