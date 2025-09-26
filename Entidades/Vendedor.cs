using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    public class Vendedor : BaseEntidade
    {
        [StringLength(100, MinimumLength = 3)]
        [Required(ErrorMessage = "Nome é obrigatório")]
        [FormField(Order = 1, DisplayName = "Nome Completo", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, Section = "Dados Básicos", Placeholder = "Digite o nome completo", GridColumns = 2)]
        public string Nome { get; set; } = "";

        [FormField(Order = 2, DisplayName = "CPF", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf, Required = false, Section = "Dados Básicos", GridColumns = 2)]
        public string? CPF { get; set; }

        [EmailAddress]
        [FormField(Order = 20, DisplayName = "Email", Icon = "fas fa-envelope", Type = EnumFieldType.Email, Section = "Dados Básicos", Placeholder = "vendedor@email.com", GridColumns = 2)]
        public string? Email { get; set; }

        [FormField(Order = 21, DisplayName = "Telefone", Icon = "fas fa-phone", Type = EnumFieldType.Phone, Section = "Dados Básicos", GridColumns = 2)]
        public string? Telefone { get; set; }

        [FormField(Order = 22, DisplayName = "Celular", Icon = "fas fa-mobile", Type = EnumFieldType.Phone, Section = "Dados Básicos", GridColumns = 2)]
        public string? Celular { get; set; }

        [FormField(Order = 23, DisplayName = "Percentual de comissão", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Percentage, Section = "Status", GridColumns = 3)]
        public decimal? PercentualComissao { get; set; }
        
        [FormField(Order = 24, DisplayName = "Meta", Icon = "fas fa-money-bill", Type = EnumFieldType.Currency, Section = "Status", GridColumns = 3)]
        public decimal? Meta { get; set; }

        [FormField(Order = 25, DisplayName = "Ativo", Type = EnumFieldType.Checkbox, Section = "Status", GridColumns = 3)]
        public bool Ativo { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
        public virtual ICollection<Tarefa> Tarefas { get; set; } = [];
    }
}