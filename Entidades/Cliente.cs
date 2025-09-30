using AutoGestao.Attributes;
using AutoGestao.Controllers;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Cliente", Subtitle = "Gerencie as informações dos clientes", Icon = "fas fa-user", EnableAjaxSubmit = true)]
    public class Cliente : BaseEntidadeEmpresa
    {
        [FormField(Order = 1, Name = "Tipo de Cliente", Section = "Tipo Registro", Icon = "fas fa-user-tag", Type = EnumFieldType.Select, Required = true, GridColumns = 1)]
        public EnumTipoPessoa TipoCliente { get; set; }

        [FormField(Order = 10, Name = "Nome Completo", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string Nome { get; set; }

        [ConditionalDisplay("TipoCliente == 1")]
        [FormField(Order = 10, Name = "Data de nascimento", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Date)]
        public DateTime? DataNascimento { get; set; }

        [ConditionalDisplay("TipoCliente == 1")]
        [FormField(Order = 10, Name = "CPF", Section = "Dados Básicos", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf)]
        public string? Cpf { get; set; }

        [ConditionalDisplay("TipoCliente == 1")]
        [FormField(Order = 10, Name = "RG", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Text)]
        public string? Rg { get; set; }

        [ConditionalDisplay("TipoCliente == 2")]
        [FormField(Order = 10, Name = "CNPJ", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Cnpj)]
        public string? Cnpj { get; set; }

        [FormField(Order = 20, Name = "Email", Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email, GridColumns = 2)]
        public string? Email { get; set; }

        [FormField(Name = "Telefone", Section = "Contato", Icon = "fas fa-phone", Type = EnumFieldType.Phone, Order = 21)]
        public string? Telefone { get; set; }

        [ConditionalRequired("IsEmpty(Telefone)", "Celular é obrigatório quando não há telefone")]
        [FormField(Name = "Celular", Section = "Contato", Icon = "fas fa-mobile", Type = EnumFieldType.Phone, Order = 22)]
        public string? Celular { get; set; }

        [FormField(Order = 30, Name = "CEP", Section = "Endereço", Icon = "fas fa-mail-bulk", Type = EnumFieldType.Cep, GridColumns = 3)]
        public string? CEP { get; set; }

        [FormField(Order = 30, Name = "Estado", Section = "Endereço", Icon = "fas fa-flag", Type = EnumFieldType.Select)]
        public EnumEstado Estado { get; set; }

        [FormField(Order = 30, Name = "Cidade", Section = "Endereço", Icon = "fas fa-city", Type = EnumFieldType.Text)]
        public string? Cidade { get; set; }

        [FormField(Order = 30, Name = "Endereço", Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text)]
        public string? Endereco { get; set; }

        [FormField(Order = 30, Name = "Número", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Numero { get; set; }

        [FormField(Order = 30, Name = "Bairro", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Bairro { get; set; }

        [FormField(Order = 30, Name = "Complemento", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Complemento { get; set; }
        
        [FormField(Order = 40, Name = "Ativo", Section = "Status", Icon = "fas fa-toggle-on", Type = EnumFieldType.Checkbox, GridColumns = 1)]
        public bool Ativo { get; set; } = true;

        [FormField(Order = 50, Name = "Observações", Section = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Informações adicionais...", GridColumns = 1)]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual ICollection<Veiculo> Veiculos { get; set; } = [];
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    }
}