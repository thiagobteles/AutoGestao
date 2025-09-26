namespace AutoGestao.Configuration
{
    /// <summary>
    /// Configurações globais para automação de Enums
    /// </summary>
    public static class EnumAutomationConfig
    {
        /// <summary>
        /// Se deve incluir ícones por padrão nos Enums
        /// </summary>
        public static bool IncludeIconsByDefault { get; set; } = true;

        /// <summary>
        /// Se deve incluir opção vazia nos selects de Enums nullable
        /// </summary>
        public static bool IncludeEmptyOptionForNullable { get; set; } = true;

        /// <summary>
        /// Texto padrão para opção vazia
        /// </summary>
        public static string EmptyOptionText { get; set; } = "Selecione...";

        /// <summary>
        /// Configurações específicas por tipo de Enum
        /// </summary>
        public static Dictionary<Type, EnumConfig> EnumConfigurations { get; set; } = [];

        /// <summary>
        /// Enums que devem ser ignorados pela automação (se houver)
        /// </summary>
        public static HashSet<Type> IgnoreEnumTypes { get; set; } = [];

        /// <summary>
        /// Propriedades que devem ser ignoradas pela automação
        /// Formato: "EntityType.PropertyName"
        /// </summary>
        public static HashSet<string> IgnoreProperties { get; set; } = [];

        /// <summary>
        /// Obtém a configuração para um tipo de Enum específico
        /// </summary>
        /// <param name="enumType">Tipo do Enum</param>
        /// <returns>Configuração do Enum</returns>
        public static EnumConfig GetEnumConfig(Type enumType)
        {
            if (EnumConfigurations.TryGetValue(enumType, out var config))
            {
                return config;
            }

            // Retorna configuração padrão
            return new EnumConfig
            {
                IncludeIcons = IncludeIconsByDefault,
                EmptyOptionText = EmptyOptionText,
                SortOrder = EnumSortOrder.ByValue
            };
        }

        /// <summary>
        /// Verifica se um Enum deve ser ignorado
        /// </summary>
        /// <param name="enumType">Tipo do Enum</param>
        /// <returns>True se deve ser ignorado</returns>
        public static bool ShouldIgnoreEnumType(Type enumType)
        {
            return IgnoreEnumTypes.Contains(enumType);
        }

        /// <summary>
        /// Verifica se uma propriedade deve ser ignorada
        /// </summary>
        /// <param name="entityType">Tipo da entidade</param>
        /// <param name="propertyName">Nome da propriedade</param>
        /// <returns>True se deve ser ignorada</returns>
        public static bool ShouldIgnoreProperty(Type entityType, string propertyName)
        {
            var key = $"{entityType.Name}.{propertyName}";
            return IgnoreProperties.Contains(key);
        }
    }

    /// <summary>
    /// Configuração específica para um Enum
    /// </summary>
    public class EnumConfig
    {
        /// <summary>
        /// Se deve incluir ícones
        /// </summary>
        public bool IncludeIcons { get; set; } = true;

        /// <summary>
        /// Texto para opção vazia (se aplicável)
        /// </summary>
        public string EmptyOptionText { get; set; } = "Selecione...";

        /// <summary>
        /// Ordem de classificação dos itens
        /// </summary>
        public EnumSortOrder SortOrder { get; set; } = EnumSortOrder.ByValue;

        /// <summary>
        /// Se deve incluir opção vazia (para Enums nullable)
        /// </summary>
        public bool IncludeEmptyOption { get; set; } = true;

        /// <summary>
        /// Filtro customizado para valores (se quiser excluir alguns valores)
        /// </summary>
        public Func<object, bool>? ValueFilter { get; set; }

        /// <summary>
        /// Transformação customizada do texto (se quiser modificar as descrições)
        /// </summary>
        public Func<string, string>? TextTransform { get; set; }
    }

    /// <summary>
    /// Ordem de classificação para Enums
    /// </summary>
    public enum EnumSortOrder
    {
        /// <summary>
        /// Manter ordem original (por valor numérico)
        /// </summary>
        ByValue,

        /// <summary>
        /// Ordenar por descrição (alfabética)
        /// </summary>
        ByDescription,

        /// <summary>
        /// Ordenar por nome da propriedade
        /// </summary>
        ByName
    }
}