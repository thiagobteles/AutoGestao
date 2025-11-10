using FGT.Atributes;
using FGT.Enumerador.Gerais;
using FGT.Helpers;
using FGT.Models;

namespace FGT.Extensions
{
    /// <summary>
    /// Extensões para FormFieldViewModel com suporte a Reference
    /// </summary>
    public static class FormFieldViewModelExtensions
    {
        /// <summary>
        /// Configura um campo de referência com base no tipo
        /// </summary>
        /// <param name="field">Campo a ser configurado</param>
        /// <param name="referenceType">Tipo da entidade de referência</param>
        /// <returns>Campo configurado</returns>
        public static FormFieldViewModel ConfigureReference(this FormFieldViewModel field, Type referenceType)
        {
            if (field.Type != EnumFieldType.Reference)
            {
                return field;
            }

            field.Reference = referenceType;
            field.ReferenceConfig = ReferenceFieldConfig.GetDefault(referenceType);

            // Configurações genéricas por convenção
            field.ReferenceConfig.ControllerName = ControllerNameHelper.GetControllerName(referenceType);
            field.ReferenceConfig.DisplayField = GetDisplayField(referenceType);
            field.ReferenceConfig.SearchFields = GetSearchFields(referenceType);
            field.ReferenceConfig.SubtitleFields = GetSubtitleFields(referenceType);

            return field;
        }

        /// <summary>
        /// Obtém o campo de exibição principal baseado em atributos
        /// </summary>
        private static string GetDisplayField(Type referenceType)
        {
            // Buscar propriedade com [ReferenceText] ou [GridMain]
            var property = referenceType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(ReferenceTextAttribute), false).Any() ||
                                   p.GetCustomAttributes(typeof(GridMainAttribute), false).Any());

            return property?.Name ?? "Id";
        }

        /// <summary>
        /// Obtém os campos de busca baseado em atributos
        /// </summary>
        private static List<string> GetSearchFields(Type referenceType)
        {
            // Buscar propriedades com [ReferenceSearchable]
            return [.. referenceType.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ReferenceSearchableAttribute), false).Any())
                .Select(p => p.Name)];
        }

        /// <summary>
        /// Obtém os campos de subtitle baseado em atributos
        /// </summary>
        private static List<string> GetSubtitleFields(Type referenceType)
        {
            // Buscar propriedades com [ReferenceSubtitle]
            return [.. referenceType.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ReferenceSubtitleAttribute), false).Any())
                .Select(p => p.Name)];
        }

        /// <summary>
        /// Gera o placeholder para um campo de referência baseado nos campos ReferenceSearchable
        /// </summary>
        /// <param name="referenceType">Tipo da entidade de referência</param>
        /// <returns>Placeholder gerado automaticamente</returns>
        public static string GetReferencePlaceholder(Type referenceType)
        {
            // Buscar propriedades com [ReferenceSearchable]
            var searchableProperties = referenceType.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ReferenceSearchableAttribute), false).Any())
                .ToList();

            if (searchableProperties.Count == 0)
            {
                // Se não houver campos searchable, retornar placeholder padrão
                return "Digite para pesquisar...";
            }

            // Obter os nomes de exibição das propriedades
            var displayNames = new List<string>();
            foreach (var prop in searchableProperties)
            {
                // Buscar FormFieldAttribute para obter o nome de exibição
                var formFieldAttr = prop.GetCustomAttributes(typeof(FormFieldAttribute), false)
                    .Cast<FormFieldAttribute>()
                    .FirstOrDefault();

                var displayName = formFieldAttr?.Name ?? prop.Name;
                displayNames.Add(displayName);
            }

            // Montar o texto do placeholder
            if (displayNames.Count == 1)
            {
                return $"Digite {displayNames[0]} para filtrar";
            }
            else if (displayNames.Count == 2)
            {
                return $"Digite {displayNames[0]} ou {displayNames[1]} para filtrar";
            }
            else
            {
                var lastItem = displayNames.Last();
                var otherItems = string.Join(", ", displayNames.Take(displayNames.Count - 1));
                return $"Digite {otherItems} ou {lastItem} para filtrar";
            }
        }
    }
}