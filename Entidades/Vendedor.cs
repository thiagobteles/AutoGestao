using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    public class Vendedor : BaseEntidadeEmpresa
    {
        [GridMain("Nome Completo")]
        [FormField(Order = 1, Name = "Nome Completo", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, Placeholder = "Digite o nome completo", GridColumns = 2)]
        public string Nome { get; set; } = "";

        [GridDocument("CPF", DocumentType.CPF)]
        [FormField(Order = 1, Name = "CPF", Section = "Dados Básicos", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf, Required = false)]
        public string? CPF { get; set; }

        [GridContact("E-mail")]
        [FormField(Order = 1, Name = "Email", Section = "Dados Básicos", Icon = "fas fa-envelope", Type = EnumFieldType.Email, Placeholder = "vendedor@email.com")]
        public string? Email { get; set; }

        [GridContact("Telefone")]
        [FormField(Order = 1, Name = "Telefone", Section = "Dados Básicos", Icon = "fas fa-phone", Type = EnumFieldType.Phone)]
        public string? Telefone { get; set; }

        [GridField("Celular", IsSubtitle = true, SubtitleOrder = 3, Order = 65)]
        [FormField(Order = 1, Name = "Celular", Section = "Dados Básicos", Icon = "fas fa-mobile", Type = EnumFieldType.Phone)]
        public string? Celular { get; set; }

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