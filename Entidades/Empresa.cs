using AutoGestao.Atributes;
using AutoGestao.Atributes;
using AutoGestao.Controllers;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Empresa", Subtitle = "Gerencie as informações das empresas", Icon = "fas fa-user", EnableAjaxSubmit = true)]
    public class Empresa : BaseEntidade
    {
        [GridMain("Razão Social")]
        [FormField(Order = 1, Name = "Razão Social", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, Placeholder = "Digite a razão social", GridColumns = 2)]
        public string RazaoSocial { get; set; } = "";

        [GridDocument("CNPJ", DocumentType.CNPJ)]
        [FormField(Order = 1, Name = "CNPJ", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Cnpj, Required = true)]
        public string? Cnpj { get; set; }

        [GridContact("Telefone")]
        [FormField(Order = 10, Name = "Telefone", Section = "Contato", Icon = "fas fa-phone", Type = EnumFieldType.Phone, GridColumns = 3)]
        public string? Telefone { get; set; }

        [GridField("Celular", IsSubtitle = true, SubtitleOrder = 2, Order = 65)]
        [FormField(Order = 10, Name = "Celular", Section = "Contato", Icon = "fas fa-mobile", Type = EnumFieldType.Phone)]
        public string? Celular { get; set; }

        [GridContact("E-mail")]
        [FormField(Order = 10, Name = "Email", Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email, Placeholder = "empresa@email.com")]
        public string? Email { get; set; }

        [GridField("CEP", Order = 70, Width = "100px", Format = "#####-###")]
        [FormField(Order = 20, Name = "CEP", Section = "Endereço", Icon = "fas fa-mail-bulk", Type = EnumFieldType.Cep, GridColumns = 3)]
        public string? CEP { get; set; }

        [GridField("Estado", Order = 75, Width = "65px")]
        [FormField(Order = 20, Name = "Estado", Section = "Endereço", Icon = "fas fa-flag", Type = EnumFieldType.Select)]
        public EnumEstado Estado { get; set; }

        [GridField("Cidade", Order = 72)]
        [FormField(Order = 20, Name = "Cidade", Section = "Endereço", Icon = "fas fa-city", Type = EnumFieldType.Text)]
        public string? Cidade { get; set; }

        [FormField(Order = 20, Name = "Endereço", Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text)]
        public string? Endereco { get; set; }

        [FormField(Order = 20, Name = "Número", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Numero { get; set; }

        [FormField(Order = 20, Name = "Bairro", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Bairro { get; set; }

        [FormField(Order = 20, Name = "Complemento", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.TextArea, Placeholder = "Complemento do endereço...")]
        public string? Complemento { get; set; }

        [FormField(Order = 30, Name = "Observações", Section = "Status", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Informações adicionais sobre a empresa...")]
        public string? Observacoes { get; set; }
    }
}