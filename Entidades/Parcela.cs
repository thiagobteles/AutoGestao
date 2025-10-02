using AutoGestao.Attributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Parcela", Subtitle = "Gerencie as parcelas de vendas", Icon = "fas fa-file-invoice-dollar", EnableAjaxSubmit = true)]
    public class Parcela : BaseEntidadeEmpresa
    {
        [GridField("Nº", Order = 10, Width = "60px")]
        [FormField(Order = 1, Name = "Número da Parcela", Section = "Identificação", Icon = "fas fa-hashtag", Type = EnumFieldType.Number, Required = true, ReadOnly = true, GridColumns = 3)]
        public int NumeroParcela { get; set; }

        [GridField("Valor", Order = 20, Width = "120px")]
        [FormField(Order = 10, Name = "Valor", Section = "Valores", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency, Required = true, GridColumns = 2)]
        public decimal Valor { get; set; }

        [GridField("Vencimento", Order = 30, Width = "110px")]
        [FormField(Order = 10, Name = "Data de Vencimento", Section = "Valores", Icon = "fas fa-calendar", Type = EnumFieldType.Date, Required = true)]
        public DateTime DataVencimento { get; set; }

        [GridField("Pagamento", Order = 35, Width = "110px")]
        [FormField(Order = 20, Name = "Data de Pagamento", Section = "Pagamento", Icon = "fas fa-calendar-check", Type = EnumFieldType.Date, GridColumns = 2)]
        public DateTime? DataPagamento { get; set; }

        [GridField("Valor Pago", Order = 40, Width = "120px")]
        [FormField(Order = 20, Name = "Valor Pago", Section = "Pagamento", Icon = "fas fa-money-bill", Type = EnumFieldType.Currency)]
        public decimal? ValorPago { get; set; }

        [GridField("Status", Order = 50, Width = "100px")]
        [FormField(Order = 30, Name = "Status", Section = "Status", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, GridColumns = 3)]
        public EnumStatusParcela Status { get; set; } = EnumStatusParcela.Pendente;

        [FormField(Order = 40, Name = "Observações", Section = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações sobre a parcela...")]
        public string? Observacoes { get; set; }

        [FormField(Order = 50, Name = "Venda", Section = "Vínculo", Icon = "fas fa-shopping-cart", Type = EnumFieldType.Reference, Reference = typeof(Venda), Required = true, ReadOnly = true)]
        public long IdVenda { get; set; }

        // Navigation properties
        public virtual Venda Venda { get; set; }
    }
}