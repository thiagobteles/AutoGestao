using AutoGestao.Enumerador.Gerais;
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

            // Configurações específicas por tipo
            field.ReferenceConfig.ControllerName = GetControllerName(referenceType);
            field.ReferenceConfig.DisplayField = GetDisplayField(referenceType);
            field.ReferenceConfig.SearchFields = GetSearchFields(referenceType);
            field.ReferenceConfig.SubtitleFields = GetSubtitleFields(referenceType);

            return field;
        }

        /// <summary>
        /// Obtém o nome do controller baseado no tipo da entidade
        /// </summary>
        private static string GetControllerName(Type referenceType)
        {
            var typeName = referenceType.Name;

            // Mapeamento específico para controllers conhecidos
            var controllerMap = new Dictionary<string, string>
            {
                ["Cliente"] = "Clientes",
                ["Fornecedor"] = "Fornecedores",
                ["Vendedor"] = "Vendedores",
                ["VeiculoMarca"] = "VeiculoMarcas",
                ["VeiculoMarcaModelo"] = "VeiculoMarcaModelos",
                ["VeiculoCor"] = "VeiculoCores",
                ["Usuario"] = "Usuarios"
            };

            return controllerMap.TryGetValue(typeName, out var controller)
                ? controller
                : typeName + "s"; // Convenção padrão: nome + s
        }

        /// <summary>
        /// Obtém o campo principal para exibição
        /// </summary>
        private static string GetDisplayField(Type referenceType)
        {
            var properties = referenceType.GetProperties();

            // Prioridade para campos de exibição
            var displayCandidates = new[] { "Nome", "Descricao", "Titulo", "RazaoSocial", "Codigo" };

            foreach (var candidate in displayCandidates)
            {
                if (properties.Any(p => p.Name == candidate && p.PropertyType == typeof(string)))
                {
                    return candidate;
                }
            }

            return "Id"; // Fallback
        }

        /// <summary>
        /// Obtém os campos para busca
        /// </summary>
        private static List<string> GetSearchFields(Type referenceType)
        {
            var fields = new List<string>();
            var properties = referenceType.GetProperties();

            // Campos comuns para busca
            var searchCandidates = new[] { "Nome", "Descricao", "CPF", "CNPJ", "Email", "Codigo", "Placa" };

            foreach (var candidate in searchCandidates)
            {
                if (properties.Any(p => p.Name == candidate && p.PropertyType == typeof(string)))
                {
                    fields.Add(candidate);
                }
            }

            return fields.Any() ? fields : ["Nome"];
        }

        /// <summary>
        /// Obtém os campos para subtitle
        /// </summary>
        private static List<string> GetSubtitleFields(Type referenceType)
        {
            var fields = new List<string>();
            var properties = referenceType.GetProperties();

            // Campos para informações adicionais
            var subtitleCandidates = new[] { "CPF", "CNPJ", "Email", "Telefone", "Codigo" };

            foreach (var candidate in subtitleCandidates)
            {
                if (properties.Any(p => p.Name == candidate && p.PropertyType == typeof(string)))
                {
                    fields.Add(candidate);
                }
            }

            return fields;
        }
    }
}
