using AutoGestao.Atributes;
using AutoGestao.Entidades.Base;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Obrigação Fiscal", Subtitle = "Controle de obrigações acessórias e prazos fiscais", Icon = "fas fa-tasks")]
    public class ObrigacaoFiscal : BaseEntidade
    {
        [FormField(Name = "Empresa Cliente", Order = 10, Section = "Dados Principais", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, Reference = typeof(EmpresaCliente))]
        [Required]
        public long EmpresaClienteId { get; set; }

        [GridComposite("Empresa", Order = 10, NavigationPaths = new[] { "EmpresaCliente.RazaoSocial", "EmpresaCliente.CNPJ" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">{1}</div></div>")]
        public string EmpresaClienteNome => $"{EmpresaCliente?.RazaoSocial ?? "N/A"} - {EmpresaCliente?.CNPJ ?? "N/A"}";

        [ReferenceText]
        [GridField("Tipo", Order = 15, Width = "180px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Tipo de Obrigação", Order = 15, Section = "Dados Principais", Icon = "fas fa-file-alt", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumTipoObrigacaoFiscal TipoObrigacao { get; set; }

        [GridField("Periodicidade", Order = 20, Width = "120px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Periodicidade", Order = 20, Section = "Dados Principais", Icon = "fas fa-calendar-alt", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumPeriodicidade Periodicidade { get; set; }

        [ReferenceSubtitle(Order = 0, Prefix = "Competência: ", Format = "MM/yyyy")]
        [GridField("Competência", Order = 25, Width = "120px", Format = "MM/yyyy")]
        [FormField(Name = "Mês/Ano de Competência", Order = 25, Section = "Período", Icon = "fas fa-calendar", Type = EnumFieldType.Date, Required = true)]
        [Required]
        public DateTime Competencia { get; set; }

        [ReferenceSubtitle(Order = 1, Prefix = "Vencimento: ", Format = "dd/MM/yyyy")]
        [GridField("Vencimento", Order = 30, Width = "120px", Format = "dd/MM/yyyy")]
        [FormField(Name = "Data de Vencimento", Order = 30, Section = "Período", Icon = "fas fa-calendar-times", Type = EnumFieldType.Date, Required = true)]
        [Required]
        public DateTime DataVencimento { get; set; }

        [ReferenceSubtitle(Order = 2, Prefix = "Status: ")]
        [GridField("Status", Order = 35, Width = "130px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Status", Order = 35, Section = "Situação", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumStatusObrigacao Status { get; set; } = EnumStatusObrigacao.Pendente;

        [GridField("Entregue em", Order = 40, Width = "120px", Format = "dd/MM/yyyy HH:mm")]
        [FormField(Name = "Data de Entrega", Order = 40, Section = "Situação", Icon = "fas fa-check-circle", Type = EnumFieldType.DateTime, ReadOnly = true)]
        public DateTime? DataEntrega { get; set; }

        [FormField(Name = "Número do Recibo", Order = 45, Section = "Protocolo", Icon = "fas fa-receipt", Type = EnumFieldType.Text, Placeholder = "Número do protocolo/recibo...")]
        [MaxLength(100)]
        public string? NumeroRecibo { get; set; }

        [FormField(Name = "Arquivo Enviado", Order = 50, Section = "Arquivos", Icon = "fas fa-file-upload", Type = EnumFieldType.File, AllowedExtensions = "xml,txt,zip", MaxSizeMB = 10)]
        [MaxLength(500)]
        public string? ArquivoEnviado { get; set; }

        [FormField(Name = "Recibo/Protocolo", Order = 55, Section = "Arquivos", Icon = "fas fa-file-pdf", Type = EnumFieldType.File, AllowedExtensions = "pdf", MaxSizeMB = 5)]
        [MaxLength(500)]
        public string? ArquivoRecibo { get; set; }

        [FormField(Name = "Observações", Order = 60, Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações sobre esta obrigação...", GridColumns = 1)]
        [MaxLength(1000)]
        public string? Observacoes { get; set; }

        // Navigation
        [ForeignKey("EmpresaClienteId")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }
    }
}
