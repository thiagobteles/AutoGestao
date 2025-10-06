namespace AutoGestao.Atributes
{
    /// <summary>
    /// Define um campo composto na grid que combina múltiplas propriedades
    /// Exemplo: Combinar Marca + Modelo em uma única coluna
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GridCompositeAttribute(string displayName) : GridFieldAttribute(displayName)
    {
        /// <summary>
        /// Propriedades de navegação a serem combinadas
        /// Exemplo: new[] { "VeiculoMarca.Descricao", "VeiculoMarcaModelo.Descricao" }
        /// </summary>
        public string[] NavigationPaths { get; set; } = [];

        /// <summary>
        /// Separador entre os valores (padrão: " - ")
        /// </summary>
        public string Separator { get; set; } = " - ";

        /// <summary>
        /// Template para formatação customizada
        /// Placeholders: {0}, {1}, {2}, etc.
        /// Exemplo: "{0} {1}/{2}" para "Marca Modelo/Ano"
        /// </summary>
        public string? Template { get; set; }
    }
}