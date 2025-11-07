using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "Item da Nota Fiscal", Subtitle = "Itens e produtos da nota fiscal", Icon = "fas fa-list")]
    public class NotaFiscalItem : BaseEntidade
    {
        [Required]
        public int NotaFiscalId { get; set; }

        [FormField(Name = "Descrição", Order = 10, Section = "Produto/Serviço", Icon = "fas fa-tag", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [FormField(Name = "Código", Order = 15, Section = "Produto/Serviço", Icon = "fas fa-barcode", Type = EnumFieldType.Text)]
        [MaxLength(50)]
        public string? Codigo { get; set; }

        [FormField(Name = "NCM", Order = 20, Section = "Produto/Serviço", Icon = "fas fa-qrcode", Type = EnumFieldType.Text)]
        [MaxLength(8)]
        public string? NCM { get; set; }

        [FormField(Name = "CFOP", Order = 25, Section = "Fiscal", Icon = "fas fa-file-contract", Type = EnumFieldType.Text)]
        [MaxLength(4)]
        public string? CFOP { get; set; }

        [FormField(Name = "Quantidade", Order = 30, Section = "Valores", Icon = "fas fa-cubes", Type = EnumFieldType.Decimal, Required = true)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantidade { get; set; } = 1;

        [FormField(Name = "Valor Unitário", Order = 35, Section = "Valores", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Decimal, Required = true)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorUnitario { get; set; }

        [FormField(Name = "Valor Total", Order = 40, Section = "Valores", Icon = "fas fa-calculator", Type = EnumFieldType.Decimal, Readonly = true)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        // Navigation
        [ForeignKey("NotaFiscalId")]
        public virtual NotaFiscal? NotaFiscal { get; set; }
    }
}
