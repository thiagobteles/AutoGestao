using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Produto", Subtitle = "Gerencie o catálogo de produtos", Icon = "fas fa-box", EnableAjaxSubmit = true)]
    [FormTabs(EnableTabs = true, DefaultTab = "principal")]
    [FormTab("historico", "Histórico", TabIcon = "fas fa-history", Order = 1, Controller = "ProdutoHistorico")]
    [FormTab("fornecedores", "Fornecedores", TabIcon = "fas fa-truck", Order = 2, Controller = "ProdutoFornecedores", RequiredRoles = new[] { "Admin", "Gerente" })]
    public class Produto : BaseEntidade
    {
        [Required]
        [FormField(DisplayName = "Código", Icon = "fas fa-barcode", Type = FormFieldType.Text, Required = true, Order = 1, Section = "Identificação")]
        public string Codigo { get; set; } = "";

        [Required]
        [StringLength(200)]
        [FormField(DisplayName = "Nome do Produto", Icon = "fas fa-tag", Type = FormFieldType.Text, Required = true, Order = 2, Section = "Identificação", GridColumns = 2)]
        public string Nome { get; set; } = "";

        [FormField(DisplayName = "Categoria", Icon = "fas fa-folder", Type = FormFieldType.Select, Required = true, Order = 3, Section = "Identificação", GridColumns = 2)]
        public EnumCategoriaProduto Categoria { get; set; }

        [FormField(DisplayName = "Descrição", Icon = "fas fa-align-left", Type = FormFieldType.TextArea, Order = 10, Section = "Detalhes", Placeholder = "Descrição detalhada do produto...")]
        public string? Descricao { get; set; }

        [Range(0, double.MaxValue)]
        [FormField(DisplayName = "Preço de Custo", Icon = "fas fa-dollar-sign", Type = FormFieldType.Currency, Order = 20, Section = "Financeiro", GridColumns = 2)]
        public decimal? PrecoCusto { get; set; }

        [Range(0, double.MaxValue)]
        [FormField(DisplayName = "Preço de Venda", Icon = "fas fa-hand-holding-usd", Type = FormFieldType.Currency, Required = true, Order = 21, Section = "Financeiro", GridColumns = 2)]
        public decimal PrecoVenda { get; set; }

        [FormField(DisplayName = "Estoque Atual", Icon = "fas fa-cubes", Type = FormFieldType.Number, Order = 30, Section = "Estoque", GridColumns = 3)]
        public int EstoqueAtual { get; set; }

        [FormField(DisplayName = "Estoque Mínimo", Icon = "fas fa-exclamation-triangle", Type = FormFieldType.Number, Order = 31, Section = "Estoque", GridColumns = 3)]
        public int EstoqueMinimo { get; set; }

        [FormField(DisplayName = "Estoque Máximo", Icon = "fas fa-layer-group", Type = FormFieldType.Number, Order = 32, Section = "Estoque", GridColumns = 3)]
        public int EstoqueMaximo { get; set; }

        [FormField(DisplayName = "Ativo", Icon = "fas fa-toggle-on", Type = FormFieldType.Checkbox, Order = 40, Section = "Status")]
        public bool Ativo { get; set; } = true;

        [FormField(DisplayName = "Observações", Icon = "fas fa-sticky-note", Type = FormFieldType.TextArea, Order = 50, Section = "Observações")]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual ICollection<ItemVenda> ItensVenda { get; set; } = [];
    }
}