using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Models.Grid
{
    public class GridAction
    {
        /// <summary>
        /// Nome da ação (identificador interno)
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Nome de exibição da ação
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// Ícone da ação (classe CSS, ex: "fas fa-eye")
        /// </summary>
        public string Icon { get; set; } = "";

        /// <summary>
        /// Classes CSS adicionais para o botão
        /// </summary>
        public string CssClass { get; set; } = "";

        /// <summary>
        /// URL da ação (pode conter placeholder {id})
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// JavaScript para executar ao clicar (alternativa à URL)
        /// </summary>
        public string? OnClick { get; set; }

        /// <summary>
        /// Indica se a ação requer seleção de um item
        /// </summary>
        public bool RequiresSelection { get; set; } = false;

        /// <summary>
        /// Condição para exibir a ação baseada no item
        /// </summary>
        public Func<object, bool>? ShowCondition { get; set; }

        /// <summary>
        /// Tipo de requisição HTTP (GET, POST, PUT, DELETE)
        /// Padrão: GET
        /// </summary>
        public EnumTypeRequest Type { get; set; } = EnumTypeRequest.Get;

        /// <summary>
        /// Target do link (ex: "_blank" para abrir em nova aba)
        /// </summary>
        public string? Target { get; set; }
    }
}
