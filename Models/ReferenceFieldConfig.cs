namespace AutoGestao.Models
{
    /// <summary>
    /// Configurações específicas para campos de referência
    /// </summary>
    public class ReferenceFieldConfig
    {
        /// <summary>
        /// Controller usado para criar novos registros (padrão: auto-detectado)
        /// </summary>
        public string? ControllerName { get; set; }

        /// <summary>
        /// Action para criação (padrão: "Create")
        /// </summary>
        public string CreateAction { get; set; } = "Create";

        /// <summary>
        /// Campos a serem buscados (padrão: auto-detectado)
        /// </summary>
        public List<string> SearchFields { get; set; } = [];

        /// <summary>
        /// Campo principal para exibição (padrão: auto-detectado)
        /// </summary>
        public string? DisplayField { get; set; }

        /// <summary>
        /// Campos para subtitle (padrão: auto-detectado)
        /// </summary>
        public List<string> SubtitleFields { get; set; } = [];

        /// <summary>
        /// Tamanho da página para busca (padrão: 10)
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Minimum characters to trigger search (padrão: 2)
        /// </summary>
        public int MinSearchLength { get; set; } = 2;

        /// <summary>
        /// Permite criar novos registros (padrão: true)
        /// </summary>
        public bool AllowCreate { get; set; } = true;

        /// <summary>
        /// Filtros adicionais para a busca
        /// </summary>
        public Dictionary<string, object> SearchFilters { get; set; } = [];

        /// <summary>
        /// Configurações específicas por tipo de entidade
        /// </summary>
        public static Dictionary<Type, ReferenceFieldConfig> DefaultConfigs { get; set; } = [];

        /// <summary>
        /// Obtém configuração padrão para um tipo específico
        /// </summary>
        /// <param name="referenceType">Tipo da entidade de referência</param>
        /// <returns>Configuração padrão</returns>
        public static ReferenceFieldConfig GetDefault(Type? referenceType)
        {
            return referenceType != null && DefaultConfigs.TryGetValue(referenceType, out var config) 
                ? config
                : new ReferenceFieldConfig();
        }
    }
}
