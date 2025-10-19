namespace AutoGestao.Models
{
    public class ReferenceFieldConfig
    {
        public string? ControllerName { get; set; }
        public string CreateAction { get; set; } = "Create";
        public List<string> SearchFields { get; set; } = [];
        public string? DisplayField { get; set; }
        public List<string> SubtitleFields { get; set; } = [];
        public int PageSize { get; set; } = 10;
        public int MinSearchLength { get; set; } = 2;
        public bool AllowCreate { get; set; } = true;
        public Dictionary<string, object> SearchFilters { get; set; } = [];
        public static Dictionary<Type, ReferenceFieldConfig> DefaultConfigs { get; set; } = [];
        public string SearchUrl { get; set; } = "";
        public Dictionary<string, object> Filters { get; set; } = [];

        public static ReferenceFieldConfig GetDefault(Type referenceType)
        {
            return new ReferenceFieldConfig
            {
                ControllerName = referenceType.Name,
                DisplayField = GetDefaultDisplayField(referenceType),
                SearchFields = GetDefaultSearchFields(referenceType),
                SearchUrl = $"/{referenceType.Name}/SearchReference"
            };
        }

        private static string GetDefaultDisplayField(Type type)
        {
            // Procurar propriedades comuns para display
            var properties = type.GetProperties();

            var displayProperty = properties.FirstOrDefault(p =>
                p.Name.Equals("Nome", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Descricao", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Title", StringComparison.OrdinalIgnoreCase));

            return displayProperty?.Name ?? "Id";
        }

        private static List<string> GetDefaultSearchFields(Type type)
        {
            var properties = type.GetProperties();
            var searchFields = new List<string>();

            // Adicionar campos comuns de busca
            var commonSearchFields = new[] { "Nome", "Descricao", "Title", "Codigo", "Name" };

            foreach (var fieldName in commonSearchFields)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

                if (property != null && property.PropertyType == typeof(string))
                {
                    searchFields.Add(property.Name);
                }
            }

            return searchFields.Any() ? searchFields : ["Id"];
        }
    }
}
