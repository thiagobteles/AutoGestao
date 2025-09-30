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

        // ============================================================
        // PROPRIEDADES PARA REGRAS CONDICIONAIS AVANÇADAS
        // ============================================================

        /// <summary>
        /// Regra condicional para exibição do campo
        /// Suporta expressões complexas e múltiplas condições
        /// Exemplo: "TipoCliente == PessoaFisica AND Age(DataNascimento) >= 10"
        /// </summary>
        public string ConditionalDisplayRule { get; set; } = "";

        /// <summary>
        /// Regra condicional para tornar o campo obrigatório
        /// Exemplo: "TipoCliente == PessoaFisica"
        /// </summary>
        public string ConditionalRequiredRule { get; set; } = "";

        /// <summary>
        /// Mensagem de erro quando o campo obrigatório não for preenchido
        /// </summary>
        public string ConditionalRequiredMessage { get; set; } = "";

        /// <summary>
        /// Indica se o campo deve ser exibido baseado nas regras
        /// Avaliado no backend
        /// </summary>
        public bool ShouldDisplay { get; set; } = true;

        /// <summary>
        /// Indica se o campo é obrigatório baseado nas regras
        /// Avaliado no backend
        /// </summary>
        public bool IsConditionallyRequired { get; set; } = false;
    }
}