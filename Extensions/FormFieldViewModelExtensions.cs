using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Helpers;
using AutoGestao.Models;

namespace AutoGestao.Extensions
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
    }
}