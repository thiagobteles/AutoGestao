using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "Nota Fiscal", Subtitle = "Gerencie as notas fiscais eletrônicas", Icon = "fas fa-file-invoice")]
    public class NotaFiscal : BaseEntidade
    {
        [ReferenceText]
        [GridField("Número", Order = 10, Width = "100px")]
        [FormField(Name = "Número da NF", Order = 10, Section = "Dados da Nota", Icon = "fas fa-hashtag", Type = EnumFieldType.Number, Required = true)]
        [Required]
        public int Numero { get; set; }

        [GridField("Série", Order = 15, Width = "80px")]
        [FormField(Name = "Série", Order = 15, Section = "Dados da Nota", Icon = "fas fa-list-ol", Type = EnumFieldType.Number, GridColumns = 3)]
        public int Serie { get; set; } = 1;

        [GridField("Chave de Acesso", Order = 20, ShowInGrid = false)]
        [FormField(Name = "Chave de Acesso", Order = 20, Section = "Dados da Nota", Icon = "fas fa-key", Type = EnumFieldType.Text, ReadOnly = true)]
        [MaxLength(44)]
        public string? ChaveAcesso { get; set; }

        [ReferenceSubtitle(Order = 0, Prefix = "Modelo: ")]
        [GridField("Modelo", Order = 25, Width = "100px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Modelo", Order = 25, Section = "Dados da Nota", Icon = "fas fa-file-alt", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumModeloNotaFiscal Modelo { get; set; }

        [GridField("Tipo", Order = 30, Width = "100px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Tipo", Order = 30, Section = "Dados da Nota", Icon = "fas fa-exchange-alt", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumTipoNotaFiscal Tipo { get; set; }

        [GridField("Status", Order = 35, Width = "120px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Status", Order = 35, Section = "Dados da Nota", Icon = "fas fa-info-circle", Type = EnumFieldType.Select)]
        public EnumStatusNotaFiscal Status { get; set; } = EnumStatusNotaFiscal.Rascunho;

        [FormField(Name = "Empresa", Order = 40, Section = "Dados da Nota", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, Reference = typeof(EmpresaCliente))]
        [Required]
        public long EmpresaClienteId { get; set; }

        [GridComposite("Empresa", Order = 40, NavigationPaths = new[] { "EmpresaCliente.RazaoSocial", "EmpresaCliente.CNPJ" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">{1}</div></div>")]
        public string EmpresaClienteNome => $"{EmpresaCliente?.RazaoSocial ?? "N/A"} - {EmpresaCliente?.CNPJ ?? "N/A"}";

        [GridField("Data Emissão", Order = 45, Width = "120px", Format = "dd/MM/yyyy HH:mm")]
        [FormField(Name = "Data de Emissão", Order = 45, Section = "Datas", Icon = "fas fa-calendar", Type = EnumFieldType.DateTime)]
        public DateTime? DataEmissao { get; set; }

        [FormField(Name = "Data de Saída/Entrada", Order = 50, Section = "Datas", Icon = "fas fa-calendar-check", Type = EnumFieldType.DateTime)]
        public DateTime? DataSaidaEntrada { get; set; }

        [GridField("Valor Total", Order = 55, Width = "120px", Format = "C")]
        [FormField(Name = "Valor Total", Order = 55, Section = "Valores", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Decimal, Required = true)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        [FormField(Name = "Valor de Produtos", Order = 60, Section = "Valores", Icon = "fas fa-box", Type = EnumFieldType.Decimal)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorProdutos { get; set; }

        [FormField(Name = "Valor de Serviços", Order = 65, Section = "Valores", Icon = "fas fa-cogs", Type = EnumFieldType.Decimal)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorServicos { get; set; }

        [FormField(Name = "Valor ICMS", Order = 70, Section = "Tributos", Icon = "fas fa-percentage", Type = EnumFieldType.Decimal)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorICMS { get; set; }

        [FormField(Name = "Valor IPI", Order = 75, Section = "Tributos", Icon = "fas fa-percentage", Type = EnumFieldType.Decimal)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorIPI { get; set; }

        [FormField(Name = "Valor PIS", Order = 80, Section = "Tributos", Icon = "fas fa-percentage", Type = EnumFieldType.Decimal)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorPIS { get; set; }

        [FormField(Name = "Valor COFINS", Order = 85, Section = "Tributos", Icon = "fas fa-percentage", Type = EnumFieldType.Decimal)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorCOFINS { get; set; }

        [FormField(Name = "XML da Nota", Order = 100, Section = "Arquivos", Icon = "fas fa-file-code", Type = EnumFieldType.File, AllowedExtensions = "xml", MaxSizeMB = 5)]
        [MaxLength(500)]
        public string? ArquivoXML { get; set; }

        [FormField(Name = "PDF DANFE", Order = 105, Section = "Arquivos", Icon = "fas fa-file-pdf", Type = EnumFieldType.File, AllowedExtensions = "pdf", MaxSizeMB = 10)]
        [MaxLength(500)]
        public string? ArquivoPDF { get; set; }

        [FormField(Name = "Observações", Order = 110, Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Informações complementares da nota...", GridColumns = 1)]
        [MaxLength(1000)]
        public string? Observacoes { get; set; }

        // Navigation properties
        [ForeignKey("EmpresaClienteId")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }

        public virtual ICollection<NotaFiscalItem> Itens { get; set; } = [];
    }
}
