using AutoGestao.Atributes;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Despesa", Subtitle = "Gerencie as despesas com veículos", Icon = "fas fa-file-invoice", EnableAjaxSubmit = true)]
    public class Despesa : BaseEntidade
    {
        [GridMain("Descrição")]
        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-align-left", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string? Descricao { get; set; }

        [GridField("Valor", Order = 20, Width = "120px")]
        [FormField(Order = 1, Name = "Valor", Section = "Dados Básicos", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency, Required = true)]
        public decimal Valor { get; set; }

        [GridField("NF", Order = 25, Width = "100px")]
        [FormField(Order = 10, Name = "Número da NF", Section = "Documento Fiscal", Icon = "fas fa-file-alt", Type = EnumFieldType.Text, GridColumns = 3)]
        public string? NumeroNF { get; set; }

        [GridField("Data", Order = 30, Width = "110px")]
        [FormField(Order = 20, Name = "Data da Despesa", Section = "Informações", Icon = "fas fa-calendar", Type = EnumFieldType.Date, Required = true, GridColumns = 3)]
        public DateTime DataDespesa { get; set; }

        [GridField("Status", Order = 35, Width = "100px")]
        [FormField(Order = 30, Name = "Status", Section = "Status", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, GridColumns = 3)]
        public EnumStatusDespesa Status { get; set; } = EnumStatusDespesa.Pendente;

        [FormField(Order = 40, Name = "Veículo", Section = "Vínculos", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(Veiculo), Required = true, GridColumns = 2)]
        public long IdVeiculo { get; set; }

        [FormField(Order = 40, Name = "Tipo de Despesa", Section = "Vínculos", Icon = "fas fa-tags", Type = EnumFieldType.Reference, Reference = typeof(DespesaTipo), Required = true, GridColumns = 2)]
        public long IdDespesaTipo { get; set; }

        [FormField(Order = 40, Name = "Fornecedor", Section = "Vínculos", Icon = "fas fa-truck", Type = EnumFieldType.Reference, Reference = typeof(Fornecedor), Required = true, GridColumns = 2)]
        public long IdFornecedor { get; set; }

        // Navigation properties
        public virtual Veiculo? Veiculo { get; set; }
        public virtual DespesaTipo? DespesaTipo { get; set; }
        public virtual Fornecedor? Fornecedor { get; set; }
    }
}