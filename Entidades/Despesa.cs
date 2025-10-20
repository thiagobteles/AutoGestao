using AutoGestao.Atributes;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [ReportConfig("Despesas do veículo", Icon = "fas fa-file-invoice", ShowLogo = true, ShowDate = true)]
    [FormConfig(Title = "Despesa", Subtitle = "Gerencie as despesas com veículos", Icon = "fas fa-file-invoice", EnableAjaxSubmit = true)]
    public class Despesa : BaseEntidade
    {
        [ReportField("Descricao", Order = 1, Section = "Despesa", Type = EnumReportFieldType.Table)]
        [GridMain("Descrição", Order = 1)]
        [FormField(Order = 1, Name = "Descrição", Section = "Informações", Icon = "fas fa-align-left", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string? Descricao { get; set; }

        [ReportField("Valor", Order = 2, Section = "Despesa", Type = EnumReportFieldType.Table, Format = "C")]
        [GridField("Valor", Order = 2, Width = "120px")]
        [FormField(Order = 2, Name = "Valor", Section = "Informações", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency, Required = true)]
        public decimal Valor { get; set; }

        [ReportField("Data", Order = 3, Section = "Despesa", Type = EnumReportFieldType.Table)]
        [GridField("Data", Order = 3, Width = "110px")]
        [FormField(Order = 3, Name = "Data da Despesa", Section = "Informações", Icon = "fas fa-calendar", Type = EnumFieldType.Date, Required = true, GridColumns = 3)]
        public DateTime DataDespesa { get; set; }

        [GridField("Status", Order = 4, Width = "100px")]
        [FormField(Order = 4, Name = "Status", Section = "Informações", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, GridColumns = 3)]
        public EnumStatusDespesa Status { get; set; } = EnumStatusDespesa.Pendente;

        [FormField(Order = 40, Name = "Veículo", Section = "Vínculos", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(Veiculo), Required = true, GridColumns = 2)]
        public long IdVeiculo { get; set; }

        [FormField(Order = 40, Name = "Tipo de Despesa", Section = "Vínculos", Icon = "fas fa-tags", Type = EnumFieldType.Reference, Reference = typeof(DespesaTipo), Required = true)]
        public long IdDespesaTipo { get; set; }

        [FormField(Order = 40, Name = "Fornecedor", Section = "Vínculos", Icon = "fas fa-truck", Type = EnumFieldType.Reference, Reference = typeof(Fornecedor), Required = true)]
        public long IdFornecedor { get; set; }

        [GridField("NF", Order = 40, Width = "100px")]
        [FormField(Order = 40, Name = "Número da NF", Section = "Vínculos", Icon = "fas fa-file-alt", Type = EnumFieldType.Text, GridColumns = 3)]
        public string? NumeroNF { get; set; }

        // Navigation properties
        public virtual Veiculo? Veiculo { get; set; }
        public virtual DespesaTipo? DespesaTipo { get; set; }
        public virtual Fornecedor? Fornecedor { get; set; }
    }
}