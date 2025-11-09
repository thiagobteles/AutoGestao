using AutoGestao.Atributes;
using AutoGestao.Entidades.Base;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Lançamento Contábil", Subtitle = "Registre lançamentos contábeis de débito e crédito", Icon = "fas fa-file-invoice-dollar")]
    public class LancamentoContabil : BaseEntidade
    {
        [ReferenceSubtitle(Order = 0, Prefix = "Data: ", Format = "dd/MM/yyyy")]
        [GridField("Data", Order = 10, Width = "120px", Format = "dd/MM/yyyy")]
        [FormField(Name = "Data do Lançamento", Order = 10, Section = "Dados Principais", Icon = "fas fa-calendar", Type = EnumFieldType.Date, Required = true, GridColumns = 3)]
        [Required]
        public DateTime DataLancamento { get; set; } = DateTime.Now;

        [ReferenceSubtitle(Order = 2, Prefix = "Tipo: ")]
        [GridField("Tipo", Order = 15, Width = "100px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Tipo de Lançamento", Order = 15, Section = "Dados Principais", Icon = "fas fa-exchange-alt", Type = EnumFieldType.Select, Required = true, GridColumns = 3)]
        [Required]
        public EnumTipoLancamento TipoLancamento { get; set; }

        [FormField(Name = "Empresa Cliente", Order = 20, Section = "Dados Principais", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, Reference = typeof(EmpresaCliente), GridColumns = 3)]
        [Required]
        public long EmpresaClienteId { get; set; }

        [GridComposite("Empresa", Order = 20, NavigationPaths = new[] { "EmpresaCliente.RazaoSocial", "EmpresaCliente.CNPJ" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">{1}</div></div>")]
        public string EmpresaClienteNome => $"{EmpresaCliente?.RazaoSocial ?? "N/A"} - {EmpresaCliente?.CNPJ ?? "N/A"}";

        [GridField("Conta Débito", Order = 25)]
        [FormField(Name = "Conta de Débito", Order = 25, Section = "Partidas Dobradas", Icon = "fas fa-minus-circle", Type = EnumFieldType.Reference, Required = true, Reference = typeof(PlanoContas))]
        [Required]
        public long ContaDebitoId { get; set; }

        [GridField("Conta Crédito", Order = 30)]
        [FormField(Name = "Conta de Crédito", Order = 30, Section = "Partidas Dobradas", Icon = "fas fa-plus-circle", Type = EnumFieldType.Reference, Required = true, Reference = typeof(PlanoContas))]
        [Required]
        public long ContaCreditoId { get; set; }

        [ReferenceSubtitle(Order = 1, Prefix = "Valor: R$ ")]
        [GridField("Valor", Order = 35, Width = "150px", Format = "C")]
        [FormField(Name = "Valor do Lançamento", Order = 35, Section = "Valores", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Decimal, Required = true)]
        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Valor { get; set; }

        [ReferenceText]
        [GridField("Histórico", Order = 40)]
        [FormField(Name = "Histórico", Order = 40, Section = "Descrição", Icon = "fas fa-align-left", Type = EnumFieldType.Text, Required = true, Placeholder = "Descrição do lançamento...")]
        [Required]
        [MaxLength(500)]
        public string Historico { get; set; } = string.Empty;

        [FormField(Name = "Complemento", Order = 45, Section = "Descrição", Icon = "fas fa-info-circle", Type = EnumFieldType.TextArea, Placeholder = "Informações complementares...", GridColumns = 1)]
        [MaxLength(1000)]
        public string? Complemento { get; set; }

        [GridField("Documento", Order = 50, Width = "150px")]
        [FormField(Name = "Número do Documento", Order = 50, Section = "Documento de Origem", Icon = "fas fa-file-alt", Type = EnumFieldType.Text, Placeholder = "Nota fiscal, recibo, etc.")]
        [MaxLength(50)]
        public string? NumeroDocumento { get; set; }

        [FormField(Name = "Nota Fiscal", Order = 55, Section = "Documento de Origem", Icon = "fas fa-file-invoice", Type = EnumFieldType.Reference, Reference = typeof(NotaFiscal))]
        public long? NotaFiscalId { get; set; }

        [GridField("Conciliado", Order = 60, Width = "100px")]
        [FormField(Name = "Lançamento Conciliado", Order = 60, Section = "Conciliação", Icon = "fas fa-check-circle", Type = EnumFieldType.Checkbox)]
        public bool Conciliado { get; set; } = false;

        [FormField(Name = "Data da Conciliação", Order = 65, Section = "Conciliação", Icon = "fas fa-calendar-check", Type = EnumFieldType.Date, ReadOnly = true)]
        public DateTime? DataConciliacao { get; set; }

        // Navigation properties
        [ForeignKey("EmpresaClienteId")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }

        [ForeignKey("ContaDebitoId")]
        public virtual PlanoContas? ContaDebito { get; set; }

        [ForeignKey("ContaCreditoId")]
        public virtual PlanoContas? ContaCredito { get; set; }

        [ForeignKey("NotaFiscalId")]
        public virtual NotaFiscal? NotaFiscal { get; set; }
    }
}
