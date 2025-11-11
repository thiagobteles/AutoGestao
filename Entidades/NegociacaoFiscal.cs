using FGT.Atributes;
using FGT.Entidades.Base;
using FGT.Enumerador.Gerais;
using FGT.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGT.Entidades
{
    [FormConfig(Title = "Negociação Fiscal", Subtitle = "Gerencie as negociações fiscais dos optantes", Icon = "fas fa-handshake")]
    public class NegociacaoFiscal : BaseEntidade
    {
        [GridField("Mês/Ano", Order = 10, Width = "100px")]
        [FormField(Name = "Mês/Ano do Requerimento", Order = 10, Section = "Dados Principais", Icon = "fas fa-calendar", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(7)]
        public string MesAnoRequerimento { get; set; } = string.Empty;

        [FormField(Name = "UF do Optante", Order = 15, Section = "Dados Principais", Icon = "fas fa-map-marker-alt", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(2)]
        public string UFOptante { get; set; } = string.Empty;

        [ReferenceSearchable]
        [FormField(Name = "CPF/CNPJ do Optante", Order = 20, Section = "Dados Principais", Icon = "fas fa-id-card", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(18)]
        public string CpfCnpjOptante { get; set; } = string.Empty;

        [GridField("Documento")]
        public string Documento => $"{(CpfCnpjOptante.Length <= 11 ? CpfCnpjOptante.AplicarMascaraCpf() : CpfCnpjOptante.AplicarMascaraCnpj())}";

        [ReferenceText]
        [ReferenceSearchable]
        [GridField("Nome do Optante", Order = 25)]
        [FormField(Name = "Nome do Optante", Order = 25, Section = "Dados Principais", Icon = "fas fa-user", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(200)]
        public string NomeOptante { get; set; } = string.Empty;

        [ReferenceSubtitle(Order = 0, Prefix = "Conta: ")]
        [FormField(Name = "Número da Conta da Negociação", Order = 30, Section = "Dados da Negociação", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(50)]
        public string NumeroContaNegociacao { get; set; } = string.Empty;

        [FormField(Name = "Tipo de Negociação", Order = 35, Section = "Dados da Negociação", Icon = "fas fa-tag", Type = EnumFieldType.Text)]
        [MaxLength(100)]
        public string? TipoNegociacao { get; set; }

        [FormField(Name = "Modalidade da Negociação", Order = 40, Section = "Dados da Negociação", Icon = "fas fa-layer-group", Type = EnumFieldType.TextArea)]
        [MaxLength(500)]
        public string? ModalidadeNegociacao { get; set; }

        [FormField(Name = "Situação da Negociação", Order = 45, Section = "Dados da Negociação", Icon = "fas fa-info-circle", Type = EnumFieldType.Text)]
        [MaxLength(100)]
        public string? SituacaoNegociacao { get; set; }

        [FormField(Name = "Quantidade de Parcelas Concedidas", Order = 50, Section = "Parcelas", Icon = "fas fa-list-ol", Type = EnumFieldType.Number)]
        public int? QtdeParcelasConcedidas { get; set; }

        [GridField("Parcelas em Atraso", Order = 55, Width = "100px")]
        [FormField(Name = "Quantidade de Parcelas em Atraso", Order = 55, Section = "Parcelas", Icon = "fas fa-exclamation-triangle", Type = EnumFieldType.Number)]
        public int? QtdeParcelasAtraso { get; set; }

        [FormField(Name = "Valor Consolidado", Order = 60, Section = "Valores", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorConsolidado { get; set; }

        [GridField("Valor Principal", Order = 65, Width = "150px", Format = "C")]
        [FormField(Name = "Valor do Principal", Order = 65, Section = "Valores", Icon = "fas fa-money-bill", Type = EnumFieldType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorPrincipal { get; set; }

        [GridField("Valor Multa", Order = 70, Width = "150px", Format = "C")]
        [FormField(Name = "Valor da Multa", Order = 70, Section = "Valores", Icon = "fas fa-hand-holding-usd", Type = EnumFieldType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorMulta { get; set; }

        [GridField("Valor Juros", Order = 75, Width = "150px", Format = "C")]
        [FormField(Name = "Valor dos Juros", Order = 75, Section = "Valores", Icon = "fas fa-percentage", Type = EnumFieldType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorJuros { get; set; }

        [FormField(Name = "Valor do Encargo Legal", Order = 80, Section = "Valores", Icon = "fas fa-gavel", Type = EnumFieldType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorEncargoLegal { get; set; }
    }
}
