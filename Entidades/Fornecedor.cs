using AutoGestao.Attributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Fornecedor", Subtitle = "Gerencie os fornecedores", Icon = "fas fa-truck", EnableAjaxSubmit = true)]
    public class Fornecedor : BaseEntidadeDocumento
    {
        [GridField("Endereço", Order = 70, ShowInGrid = false)]
        [FormField(Order = 30, Name = "Endereço", Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text, GridColumns = 3)]
        public string? Endereco { get; set; }

        [GridField("Cidade", Order = 72)]
        [FormField(Order = 30, Name = "Cidade", Section = "Endereço", Icon = "fas fa-city", Type = EnumFieldType.Text)]
        public string? Cidade { get; set; }

        [GridField("Estado", Order = 75, Width = "65px")]
        [FormField(Order = 30, Name = "Estado", Section = "Endereço", Icon = "fas fa-flag", Type = EnumFieldType.Select)]
        public EnumEstado Estado { get; set; }

        [GridField("CEP", Order = 77, Width = "100px", Format = "#####-###")]
        [FormField(Order = 30, Name = "CEP", Section = "Endereço", Icon = "fas fa-mail-bulk", Type = EnumFieldType.Cep)]
        public string? CEP { get; set; }

        [FormField(Order = 30, Name = "Número", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Numero { get; set; }

        [FormField(Order = 30, Name = "Complemento", Section = "Endereço", Icon = "fas fa-map-marker", Type = EnumFieldType.Text)]
        public string? Complemento { get; set; }

        [FormField(Order = 30, Name = "Bairro", Section = "Endereço", Icon = "fas fa-map", Type = EnumFieldType.Text)]
        public string? Bairro { get; set; }

        [FormField(Order = 40, Name = "Observações", Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações sobre o fornecedor...")]
        public string? Observacoes { get; set; }
    }
}