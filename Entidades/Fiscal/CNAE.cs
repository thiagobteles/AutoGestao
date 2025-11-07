using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "CNAE", Subtitle = "Classificação Nacional de Atividades Econômicas", Icon = "fas fa-industry")]
    public class CNAE : BaseEntidade
    {
        [GridField("Código", Order = 10, Width = "100px")]
        [FormField(Name = "Código", Order = 10, Section = "Dados do CNAE", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty;

        [GridField("Descrição", Order = 15)]
        [FormField(Name = "Descrição", Order = 15, Section = "Dados do CNAE", Icon = "fas fa-align-left", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [FormField(Name = "Alíquota ISS (%)", Order = 20, Section = "Tributação", Icon = "fas fa-percentage", Type = EnumFieldType.Decimal)]
        public decimal? AliquotaISS { get; set; }
    }
}
