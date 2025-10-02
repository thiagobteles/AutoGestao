using AutoGestao.Attributes;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Venda", Subtitle = "Gerencie as vendas de veículos", Icon = "fas fa-shopping-cart", EnableAjaxSubmit = true)]
    public class Venda : BaseEntidadeEmpresa
    {
        [GridField("Valor", Order = 20, Width = "120px")]
        [FormField(Order = 1, Name = "Valor da Venda", Section = "Valores", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency, Required = true, GridColumns = 2)]
        public decimal ValorVenda { get; set; }

        [GridField("Entrada", Order = 21, Width = "120px")]
        [FormField(Order = 1, Name = "Valor de Entrada", Section = "Valores", Icon = "fas fa-money-bill-wave", Type = EnumFieldType.Currency)]
        public decimal? ValorEntrada { get; set; }

        [GridField("Parcelas", Order = 22, Width = "80px")]
        [FormField(Order = 1, Name = "Número de Parcelas", Section = "Valores", Icon = "fas fa-list-ol", Type = EnumFieldType.Number)]
        public int? NumeroParcelas { get; set; }

        [GridField("Pagamento", Order = 30, Width = "120px")]
        [FormField(Order = 10, Name = "Forma de Pagamento", Section = "Pagamento", Icon = "fas fa-credit-card", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public EnumFormaPagamento FormaPagamento { get; set; }

        [GridField("Status", Order = 35, Width = "100px")]
        [FormField(Order = 10, Name = "Status da Venda", Section = "Pagamento", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true)]
        public EnumStatusVenda Status { get; set; }

        [GridField("Data", Order = 15, Width = "110px")]
        [FormField(Order = 20, Name = "Data da Venda", Section = "Informações", Icon = "fas fa-calendar", Type = EnumFieldType.Date, Required = true, GridColumns = 3)]
        public DateTime DataVenda { get; set; }

        [FormField(Order = 30, Name = "Cliente", Section = "Participantes", Icon = "fas fa-user", Type = EnumFieldType.Reference, Reference = typeof(Cliente), Required = true, GridColumns = 2)]
        public long IdCliente { get; set; }

        [FormField(Order = 30, Name = "Veículo", Section = "Participantes", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(Veiculo), Required = true, GridColumns = 2)]
        public long IdVeiculo { get; set; }

        [FormField(Order = 30, Name = "Vendedor", Section = "Participantes", Icon = "fas fa-user-tie", Type = EnumFieldType.Reference, Reference = typeof(Vendedor), Required = true, GridColumns = 2)]
        public long IdVendedor { get; set; }

        [FormField(Order = 40, Name = "Observações", Section = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Informações adicionais sobre a venda...")]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual Cliente Cliente { get; set; }
        public virtual Veiculo Veiculo { get; set; }
        public virtual Vendedor Vendedor { get; set; }
        public virtual ICollection<Parcela> Parcelas { get; set; } = [];
    }
}