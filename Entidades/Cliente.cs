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
        [FormField(Name = "Tipo de Cliente", Icon = "fas fa-user-tag", Type = EnumFieldType.Select, Required = true, Order = 1, Section = "Tipo de Cliente")]
        public EnumTipoPessoa TipoCliente { get; set; }

        [FormField(Order = 10, Name = "Nome Completo", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, Section = "Dados Básicos", Placeholder = "Digite o nome completo")]
        public string Nome { get; set; }

        [ConditionalDisplay("TipoCliente == 1")]
        [FormField(Name = "Data de nascimento", Icon = "fas fa-building", Type = EnumFieldType.Date, Order = 14, Section = "Dados Básicos")]
        public DateTime? DataNascimento { get; set; }

        [ConditionalDisplay("TipoCliente == 1")]
        [FormField(Name = "CPF", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf, Order = 11, Section = "Dados Básicos", GridColumns = 2)]
        public string? Cpf { get; set; }

        [ConditionalDisplay("TipoCliente == 1")]
        [FormField(Name = "RG", Icon = "fas fa-building", Type = EnumFieldType.Text, Order = 13, Section = "Dados Básicos")]
        public string? Rg { get; set; }

        [ConditionalDisplay("TipoCliente == 2")]
        [FormField(Name = "CNPJ", Icon = "fas fa-building", Type = EnumFieldType.Cnpj, Order = 12, Section = "Dados Básicos")]
        public string? Cnpj { get; set; }

        [FormField(Name = "Email", Icon = "fas fa-envelope", Type = EnumFieldType.Email, Order = 20, Section = "Contato", Placeholder = "cliente@email.com")]
        public string? Email { get; set; }

        [FormField(Name = "Telefone", Icon = "fas fa-phone", Type = EnumFieldType.Phone, Order = 21, Section = "Contato", GridColumns = 2)]
        public string? Telefone { get; set; }

        [ConditionalRequired("IsEmpty(Telefone)", "Celular é obrigatório quando não há telefone")]
        [FormField(Name = "Celular", Icon = "fas fa-mobile", Type = EnumFieldType.Phone, Order = 22, Section = "Contato", GridColumns = 2)]
        public string? Celular { get; set; }

        [FormField(Name = "CEP", Icon = "fas fa-mail-bulk", Type = EnumFieldType.Cep, Order = 30, Section = "Endereço", GridColumns = 3)]
        public string? CEP { get; set; }

        [FormField(Name = "Estado", Icon = "fas fa-flag", Type = EnumFieldType.Select, Order = 31, Section = "Endereço", GridColumns = 3)]
        public EnumEstado Estado { get; set; }

        [FormField(Name = "Cidade", Icon = "fas fa-city", Type = EnumFieldType.Text, Order = 32, Section = "Endereço", GridColumns = 3)]
        public string? Cidade { get; set; }

        [FormField(Name = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text, Order = 33, Section = "Endereço", GridColumns = 2)]
        public string? Endereco { get; set; }

        [FormField(Name = "Número", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Order = 34, Section = "Endereço", GridColumns = 1)]
        public string? Numero { get; set; }

        [FormField(Name = "Bairro", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Order = 35, Section = "Endereço", GridColumns = 1)]
        public string? Bairro { get; set; }

        [FormField(Name = "Complemento", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Order = 36, Section = "Endereço", GridColumns = 1)]
        public string? Complemento { get; set; }
        
        [FormField(Name = "Ativo", Icon = "fas fa-toggle-on", Type = EnumFieldType.Checkbox, Order = 40, Section = "Status")]
        public bool Ativo { get; set; } = true;

        [FormField(Name = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Order = 50, Section = "Observações", Placeholder = "Informações adicionais sobre o cliente...")]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual ICollection<Veiculo> Veiculos { get; set; } = [];
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    }
}