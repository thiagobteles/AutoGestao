using AutoGestao.Enumerador.Gerais;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoGestao.Models
{
    /// <summary>
    /// ViewModel que representa um campo do formulário
    /// </summary>
    public class FormFieldViewModel
    {
        /// <summary>
        /// Nome da propriedade no modelo
        /// </summary>
        public string PropertyName { get; set; } = "";

        /// <summary>
        /// Nome de exibição do campo
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// Ícone do campo
        /// </summary>
        public string Icon { get; set; } = "fas fa-edit";

        /// <summary>
        /// Placeholder do campo
        /// </summary>
        public string Placeholder { get; set; } = "";

        /// <summary>
        /// Tipo do campo
        /// </summary>
        public EnumFieldType Type { get; set; } = EnumFieldType.Text;

        /// <summary>
        /// Indica se o campo é obrigatório
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// Indica se o campo é somente leitura
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Valor atual do campo
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Regex para validação
        /// </summary>
        public string ValidationRegex { get; set; } = "";

        /// <summary>
        /// Mensagem de validação
        /// </summary>
        public string ValidationMessage { get; set; } = "";

        /// <summary>
        /// Campo condicional
        /// </summary>
        public string ConditionalField { get; set; } = "";

        /// <summary>
        /// Valor condicional
        /// </summary>
        public string ConditionalValue { get; set; } = "";

        /// <summary>
        /// Colunas do grid
        /// </summary>
        public int GridColumns { get; set; } = 1;

        /// <summary>
        /// Classe CSS customizada
        /// </summary>
        public string CssClass { get; set; } = "";

        /// <summary>
        /// Data list
        /// </summary>
        public string DataList { get; set; } = "";

        /// <summary>
        /// Ordem de exibição
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Seção do formulário
        /// </summary>
        public string Section { get; set; } = "Não Informado";

        /// <summary>
        /// Opções para campos Select
        /// </summary>
        public List<SelectListItem> Options { get; set; } = [];

        /// <summary>
        /// Tipo da entidade de referência
        /// </summary>
        public Type? Reference { get; set; }

        /// <summary>
        /// Configurações do campo de referência
        /// </summary>
        public ReferenceFieldConfig ReferenceConfig { get; set; } = new();
    }

    /// <summary>
    /// NOVA CLASSE: Configurações específicas para campos de referência
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
            if (referenceType != null && DefaultConfigs.TryGetValue(referenceType, out var config))
            {
                return config;
            }

            return new ReferenceFieldConfig();
        }
    }

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

    public class FormViewModel
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public string Action { get; set; } = "";
        public object Entity { get; set; } = new();
        public List<FormSectionViewModel> Sections { get; set; } = [];
        public List<FormTabViewModel> Tabs { get; set; } = [];
        public bool EnableAjaxSubmit { get; set; } = true;
        public string BackAction { get; set; } = "Index";
        public string BackText { get; set; } = "Voltar à Lista";
    }

    public class FormTabViewModel
    {
        public string TabId { get; set; } = "";
        public string TabName { get; set; } = "";
        public string TabIcon { get; set; } = "fas fa-edit";
        public int Order { get; set; } = 0;
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public bool LazyLoad { get; set; } = true;
        public bool IsActive { get; set; } = false;
        public bool HasAccess { get; set; } = true;
        public string Content { get; set; } = "";
        public Dictionary<string, object> Parameters { get; set; } = [];
    }

    public class StandardFormViewModel
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public string BackAction { get; set; } = "Index";
        public string BackText { get; set; } = "Voltar à Lista";
        public string ActionName { get; set; } = "";
        public string ControllerName { get; set; } = "";
        public object Model { get; set; }
        public List<FormSectionViewModel> Sections { get; set; } = [];
        public Dictionary<string, string> ModelState { get; set; } = [];
        public bool EnableAjaxSubmit { get; set; } = true;
        public bool IsEditMode { get; set; } = false;
        public bool IsDetailsMode { get; set; } = false;
    }

    public class FormSectionViewModel
    {
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "fas fa-edit";
        public int GridColumns { get; set; } = 1;
        public List<FormFieldViewModel> Fields { get; set; } = [];
    }

    public class TabbedFormViewModel : StandardFormViewModel
    {
        public List<FormTabViewModel> Tabs { get; set; } = [];
        public string ActiveTab { get; set; } = "principal";
        public bool EnableTabs { get; set; } = false;
        public int EntityId { get; set; } = 0;
    }
}