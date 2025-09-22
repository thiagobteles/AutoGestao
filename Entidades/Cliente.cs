using AutoGestao.Atributes;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Cliente", Subtitle = "Gerencie as informações dos clientes", Icon = "fas fa-user", EnableAjaxSubmit = true)]
    public class Cliente : BaseEntidade
    {
        [FormField(DisplayName = "Tipo de Cliente", Icon = "fas fa-user-tag", Type = FormFieldType.Select, Required = true, Order = 1, Section = "Tipo de Cliente")]
        public EnumTipoPessoa TipoCliente { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required(ErrorMessage = "Nome é obrigatório")]
        [FormField(DisplayName = "Nome Completo", Icon = "fas fa-signature", Type = FormFieldType.Text, Required = true, Order = 10, Section = "Dados Básicos", Placeholder = "Digite o nome completo")]
        public string Nome { get; set; } = "";

        [FormField(DisplayName = "CPF", Icon = "fas fa-fingerprint", Type = FormFieldType.Cpf, Required = false, Order = 11, Section = "Dados Básicos", ConditionalField = "TipoCliente", ConditionalValue = "PessoaFisica", GridColumns = 2)]
        public string? CPF { get; set; }

        [FormField(DisplayName = "RG", Icon = "fas fa-building", Type = FormFieldType.Text, Required = false, Order = 13, Section = "Dados Básicos", ConditionalField = "TipoCliente", ConditionalValue = "PessoaFisica")]
        public string? RG { get; set; }

        [FormField(DisplayName = "Data de nascimento", Icon = "fas fa-building", Type = FormFieldType.Date, Required = false, Order = 14, Section = "Dados Básicos", ConditionalField = "TipoCliente", ConditionalValue = "PessoaFisica")]
        public DateTime? DataNascimento { get; set; }

        [FormField(DisplayName = "CNPJ", Icon = "fas fa-building", Type = FormFieldType.Cnpj, Required = false, Order = 12, Section = "Dados Básicos", ConditionalField = "TipoCliente", ConditionalValue = "PessoaJuridica")]
        public string? CNPJ { get; set; }

        [EmailAddress]
        [FormField(DisplayName = "Email", Icon = "fas fa-envelope", Type = FormFieldType.Email, Order = 20, Section = "Contato", Placeholder = "cliente@email.com")]
        public string? Email { get; set; }

        [FormField(DisplayName = "Telefone", Icon = "fas fa-phone", Type = FormFieldType.Phone, Order = 21, Section = "Contato", GridColumns = 2)]
        public string? Telefone { get; set; }

        [FormField(DisplayName = "Celular", Icon = "fas fa-mobile", Type = FormFieldType.Phone, Order = 22, Section = "Contato", GridColumns = 2)]
        public string? Celular { get; set; }

        [FormField(DisplayName = "CEP", Icon = "fas fa-mail-bulk", Type = FormFieldType.Cep, Order = 30, Section = "Endereço", GridColumns = 3)]
        public string? CEP { get; set; }

        [FormField(DisplayName = "Estado", Icon = "fas fa-flag", Type = FormFieldType.Select, Order = 31, Section = "Endereço", GridColumns = 3)]
        public EnumEstado Estado { get; set; }

        [FormField(DisplayName = "Cidade", Icon = "fas fa-city", Type = FormFieldType.Text, Order = 32, Section = "Endereço", GridColumns = 3)]
        public string? Cidade { get; set; }

        [FormField(DisplayName = "Endereço", Icon = "fas fa-road", Type = FormFieldType.Text, Order = 33, Section = "Endereço", GridColumns = 2)]
        public string? Endereco { get; set; }

        [FormField(DisplayName = "Número", Icon = "fas fa-hashtag", Type = FormFieldType.Text, Order = 34, Section = "Endereço", GridColumns = 1)]
        public string? Numero { get; set; }

        [FormField(DisplayName = "Bairro", Icon = "fas fa-hashtag", Type = FormFieldType.Text, Order = 35, Section = "Endereço", GridColumns = 1)]
        public string? Bairro { get; set; }

        [FormField(DisplayName = "Complemento", Icon = "fas fa-hashtag", Type = FormFieldType.Text, Order = 36, Section = "Endereço", GridColumns = 1)]
        public string? Complemento { get; set; }
        
        [FormField(DisplayName = "Ativo", Icon = "fas fa-toggle-on", Type = FormFieldType.Checkbox, Order = 40, Section = "Status")]
        public bool Ativo { get; set; } = true;

        [FormField(DisplayName = "Observações", Icon = "fas fa-sticky-note", Type = FormFieldType.TextArea, Order = 50, Section = "Observações", Placeholder = "Informações adicionais sobre o cliente...")]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual ICollection<Veiculo> Veiculos { get; set; } = [];
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    }
}