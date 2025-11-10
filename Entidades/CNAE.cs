using FGT.Atributes;
using FGT.Entidades.Base;
using FGT.Enumerador;
using FGT.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace FGT.Entidades
{
    [FormConfig(Title = "CNAE", Subtitle = "Classificação Nacional de Atividades Econômicas", Icon = "fas fa-industry")]
    public class CNAE : BaseEntidade
    {
        [ReferenceText]
        [GridField("Código", Order = 10, Width = "100px")]
        [FormField(Name = "Código", Order = 10, Section = "Dados do CNAE", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(10)]
        public string Codigo { get; set; } = string.Empty;

        [ReferenceSubtitle(Order = 0)]
        [GridField("Descrição", Order = 15)]
        [FormField(Name = "Descrição", Order = 15, Section = "Dados do CNAE", Icon = "fas fa-align-left", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [FormField(Name = "Alíquota ISS (%)", Order = 20, Section = "Tributação", Icon = "fas fa-percentage", Type = EnumFieldType.Decimal)]
        public decimal? AliquotaISS { get; set; }
    }
}
