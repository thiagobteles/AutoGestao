using AutoGestao.Atributes;
using AutoGestao.Entidades.Base;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Plano de Contas", Subtitle = "Estrutura contábil para classificação de receitas e despesas", Icon = "fas fa-sitemap")]
    public class PlanoContas : BaseEntidade
    {
        [ReferenceText]
        [GridField("Código", Order = 10, Width = "120px")]
        [FormField(Name = "Código da Conta", Order = 10, Section = "Identificação", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Required = true, Placeholder = "Ex: 1.1.01.001", GridColumns = 4)]
        [Required]
        [MaxLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [ReferenceSubtitle(Order = 0)]
        [GridField("Descrição", Order = 15)]
        [FormField(Name = "Descrição", Order = 15, Section = "Identificação", Icon = "fas fa-align-left", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; } = string.Empty;

        [ReferenceSubtitle(Order = 1, Prefix = "Tipo: ")]
        [GridField("Tipo", Order = 20, Width = "120px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Tipo de Conta", Order = 20, Section = "Classificação", Icon = "fas fa-tags", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumTipoContaContabil TipoConta { get; set; }

        [GridField("Natureza", Order = 25, Width = "120px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Natureza", Order = 25, Section = "Classificação", Icon = "fas fa-balance-scale", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumNaturezaConta Natureza { get; set; }

        [FormField(Name = "Conta Pai", Order = 30, Section = "Hierarquia", Icon = "fas fa-level-up-alt", Type = EnumFieldType.Reference, Reference = typeof(PlanoContas), Placeholder = "Deixe em branco para conta de nível superior...")]
        public long? ContaPaiId { get; set; }

        [GridField("Nível", Order = 35, Width = "80px")]
        [FormField(Name = "Nível Hierárquico", Order = 35, Section = "Hierarquia", Icon = "fas fa-layer-group", Type = EnumFieldType.Number, GridColumns = 4, ReadOnly = true)]
        public int Nivel { get; set; } = 1;

        [GridField("Analítica", Order = 40, Width = "100px")]
        [FormField(Name = "Conta Analítica", Order = 40, Section = "Configurações", Icon = "fas fa-chart-pie", Type = EnumFieldType.Checkbox, Placeholder = "Contas analíticas podem receber lançamentos")]
        public bool ContaAnalitica { get; set; } = true;

        [FormField(Name = "Aceita Lançamento", Order = 45, Section = "Configurações", Icon = "fas fa-edit", Type = EnumFieldType.Checkbox)]
        public bool AceitaLancamento { get; set; } = true;

        [FormField(Name = "DRE (Demonstração Resultado)", Order = 50, Section = "Relatórios", Icon = "fas fa-file-invoice-dollar", Type = EnumFieldType.Checkbox, Placeholder = "Exibir esta conta na DRE")]
        public bool ExibirNaDRE { get; set; } = true;

        [FormField(Name = "Balancete", Order = 55, Section = "Relatórios", Icon = "fas fa-balance-scale-right", Type = EnumFieldType.Checkbox, Placeholder = "Exibir esta conta no balancete")]
        public bool ExibirNoBalancete { get; set; } = true;

        [FormField(Name = "Observações", Order = 60, Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações sobre esta conta...", GridColumns = 1)]
        [MaxLength(500)]
        public string? Observacoes { get; set; }

        // Navigation properties
        [ForeignKey("ContaPaiId")]
        public virtual PlanoContas? ContaPai { get; set; }

        public virtual ICollection<PlanoContas> ContasFilhas { get; set; } = [];
        public virtual ICollection<LancamentoContabil> Lancamentos { get; set; } = [];
    }
}
