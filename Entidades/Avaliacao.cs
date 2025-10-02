using AutoGestao.Attributes;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Avaliação", Subtitle = "Gerencie as avaliações de veículos", Icon = "fas fa-clipboard-check", EnableAjaxSubmit = true)]
    public class Avaliacao : BaseEntidadeEmpresa
    {
        [GridField("Ano", Order = 20, Width = "80px")]
        [FormField(Order = 1, Name = "Ano do Veículo", Section = "Dados do Veículo", Icon = "fas fa-calendar", Type = EnumFieldType.Number, Required = true, GridColumns = 3)]
        public int AnoVeiculo { get; set; }

        [GridField("Placa", Order = 25, Width = "100px")]
        [FormField(Order = 1, Name = "Placa", Section = "Dados do Veículo", Icon = "fas fa-id-card", Type = EnumFieldType.Text, Placeholder = "XXX-0000")]
        public string? PlacaVeiculo { get; set; }

        [GridField("Valor Oferecido", Order = 30, Width = "130px")]
        [FormField(Order = 10, Name = "Valor Oferecido", Section = "Avaliação", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency, GridColumns = 2)]
        public decimal? ValorOferecido { get; set; }

        [GridField("Data", Order = 35, Width = "110px")]
        [FormField(Order = 10, Name = "Data da Avaliação", Section = "Avaliação", Icon = "fas fa-calendar", Type = EnumFieldType.Date, Required = true)]
        public DateTime DataAvaliacao { get; set; }

        [GridField("Status", Order = 40, Width = "100px")]
        [FormField(Order = 10, Name = "Status", Section = "Avaliação", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true)]
        public EnumStatusAvaliacao StatusAvaliacao { get; set; }

        [FormField(Order = 20, Name = "Cliente", Section = "Participantes", Icon = "fas fa-user", Type = EnumFieldType.Reference, Reference = typeof(Cliente), GridColumns = 2)]
        public long? IdCliente { get; set; }

        [FormField(Order = 20, Name = "Vendedor Responsável", Section = "Participantes", Icon = "fas fa-user-tie", Type = EnumFieldType.Reference, Reference = typeof(Vendedor), GridColumns = 2)]
        public long? IdVendedorResponsavel { get; set; }

        [FormField(Order = 30, Name = "Marca", Section = "Veículo", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarca), GridColumns = 2)]
        public long? IdVeiculoMarca { get; set; }

        [ConditionalRule(EnumConditionalRuleType.Enabled, "IdVeiculoMarca != 0")]
        [ReferenceFilter("IdVeiculoMarca", "IdVeiculoMarca", Operator = EnumFilterOperator.Equals)]
        [FormField(Order = 30, Name = "Modelo", Section = "Veículo", Icon = "fas fa-car-side", Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarcaModelo), GridColumns = 2)]
        public long? IdVeiculoMarcaModelo { get; set; }

        [FormField(Order = 40, Name = "Observações", Section = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Informações sobre a avaliação...")]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual Cliente? Cliente { get; set; }
        public virtual Vendedor? VendedorResponsavel { get; set; }
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
        public virtual VeiculoMarcaModelo? VeiculoMarcaModelo { get; set; }
    }
}