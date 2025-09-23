using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Item de Venda", Subtitle = "Itens que compõem uma venda", Icon = "fas fa-shopping-cart")]
    public class ItemVenda : BaseEntidade
    {
        [FormField(DisplayName = "Venda", Icon = "fas fa-receipt", Type = FormFieldType.Select, Required = true, Order = 1, Section = "Venda")]
        public int VendaId { get; set; }

        [FormField(DisplayName = "Produto", Icon = "fas fa-box", Type = FormFieldType.Select, Required = true, Order = 2, Section = "Produto")]
        public int ProdutoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        [FormField(DisplayName = "Quantidade", Icon = "fas fa-hashtag", Type = FormFieldType.Number, Required = true, Order = 10, Section = "Detalhes", GridColumns = 2)]
        public int Quantidade { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor unitário deve ser maior que zero")]
        [FormField(DisplayName = "Valor Unitário", Icon = "fas fa-dollar-sign", Type = FormFieldType.Currency, Required = true, Order = 11, Section = "Detalhes", GridColumns = 2)]
        public decimal ValorUnitario { get; set; }

        [Range(0, 100)]
        [FormField(DisplayName = "Desconto (%)", Icon = "fas fa-percentage", Type = FormFieldType.Percentage, Order = 12, Section = "Detalhes", GridColumns = 2)]
        public decimal PercentualDesconto { get; set; } = 0;

        [FormField(DisplayName = "Valor do Desconto", Icon = "fas fa-minus-circle", Type = FormFieldType.Currency, ReadOnly = true, Order = 13, Section = "Detalhes", GridColumns = 2)]
        public decimal ValorDesconto { get; private set; }

        [FormField(DisplayName = "Valor Total", Icon = "fas fa-calculator", Type = FormFieldType.Currency, ReadOnly = true, Order = 14, Section = "Totais")]
        public decimal ValorTotal { get; private set; }

        [FormField(DisplayName = "Observações", Icon = "fas fa-sticky-note", Type = FormFieldType.TextArea, Order = 20, Section = "Observações")]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual Venda Venda { get; set; } = null!;
        public virtual Produto Produto { get; set; } = null!;

        // Método para calcular valores
        public void CalcularValores()
        {
            ValorDesconto = (ValorUnitario * Quantidade) * (PercentualDesconto / 100);
            ValorTotal = (ValorUnitario * Quantidade) - ValorDesconto;
        }
    }
}