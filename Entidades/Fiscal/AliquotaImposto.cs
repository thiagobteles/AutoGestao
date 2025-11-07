using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "Alíquota de Imposto", Subtitle = "Gerencie as alíquotas de impostos para cálculos fiscais", Icon = "fas fa-percentage")]
    public class AliquotaImposto : BaseEntidade
    {
        [GridField("Tipo de Imposto", Order = 10, Width = "150px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Tipo de Imposto", Order = 10, Section = "Dados Principais", Icon = "fas fa-file-invoice-dollar", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumTipoImposto TipoImposto { get; set; }

        [GridField("Alíquota (%)", Order = 15, Width = "100px")]
        [FormField(Name = "Alíquota (%)", Order = 15, Section = "Dados Principais", Icon = "fas fa-percentage", Type = EnumFieldType.Decimal, Required = true)]
        [Column(TypeName = "decimal(5,2)")]
        [Required]
        public decimal AliquotaPercentual { get; set; }

        [GridField("Regime Tributário", Order = 20, Width = "150px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Regime Tributário", Order = 20, Section = "Dados Principais", Icon = "fas fa-balance-scale", Type = EnumFieldType.Select)]
        public EnumRegimeTributario? RegimeTributario { get; set; }

        [GridField("Estado", Order = 25, Width = "80px")]
        [FormField(Name = "Estado (UF)", Order = 25, Section = "Localização", Icon = "fas fa-flag", Type = EnumFieldType.Select, GridColumns = 4, Placeholder = "Todos os estados...")]
        public EnumEstado? Estado { get; set; }

        [FormField(Name = "Cidade", Order = 30, Section = "Localização", Icon = "fas fa-city", Type = EnumFieldType.Text, Placeholder = "Deixe em branco para todas as cidades...")]
        [MaxLength(100)]
        public string? Cidade { get; set; }

        [GridField("Vigência Inicial", Order = 35, Width = "120px", Format = "dd/MM/yyyy")]
        [FormField(Name = "Data de Vigência Inicial", Order = 35, Section = "Período de Vigência", Icon = "fas fa-calendar-check", Type = EnumFieldType.Date, Required = true)]
        [Required]
        public DateTime DataVigenciaInicial { get; set; } = DateTime.Now;

        [GridField("Vigência Final", Order = 40, Width = "120px", Format = "dd/MM/yyyy")]
        [FormField(Name = "Data de Vigência Final", Order = 40, Section = "Período de Vigência", Icon = "fas fa-calendar-times", Type = EnumFieldType.Date, Placeholder = "Deixe em branco para vigência indeterminada...")]
        public DateTime? DataVigenciaFinal { get; set; }

        [FormField(Name = "Observações", Order = 50, Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações sobre esta alíquota...", GridColumns = 1)]
        [MaxLength(500)]
        public string? Observacoes { get; set; }
    }
}
