using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    public class Vendedor : BaseEntidadeDocumento
    {
        [GridField("Comissão %", Order = 85, Width = "100px")]
        [FormField(Order = 10, Name = "Percentual de comissão", Section = "Status", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Percentage, GridColumns = 3)]
        public decimal? PercentualComissao { get; set; }

        [GridField("Meta", Order = 86, Width = "120px")]
        [FormField(Order = 10, Name = "Meta", Section = "Status", Icon = "fas fa-money-bill", Type = EnumFieldType.Currency)]
        public decimal? Meta { get; set; }

        // Navigation properties
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
        public virtual ICollection<Tarefa> Tarefas { get; set; } = [];
    }
}