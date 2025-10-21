using AutoGestao.Atributes;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Cliente", Subtitle = "Gerencie as informações dos clientes", Icon = "fas fa-user", EnableAjaxSubmit = true)]
    public class Cliente : BaseEntidadeDocumento
    {
        [GridField("CEP", Order = 70, Width = "100px", Format = "#####-###", ShowInGrid = false)]
        [FormField(Name = "CEP", Order = 30, Section = "Endereço", Icon = "fas fa-mail-bulk", Type = EnumFieldType.Cep, GridColumns = 3)]
        public string? CEP { get; set; }

        [GridField("UF", Order = 75, Width = "65px", EnumRender = EnumRenderType.Description)]
        [FormField(Name = "Estado", Order = 30, Section = "Endereço", Icon = "fas fa-flag", Type = EnumFieldType.Select)]
        public EnumEstado Estado { get; set; }

        [GridField("Cidade", Order = 72)]
        [FormField(Name = "Cidade", Order = 30, Section = "Endereço", Icon = "fas fa-city", Type = EnumFieldType.Text)]
        public string? Cidade { get; set; }

        [FormField(Name = "Endereço", Order = 30, Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text)]
        public string? Endereco { get; set; }

        [FormField(Name = "Número", Order = 30, Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Numero { get; set; }

        [FormField(Name = "Bairro", Order = 30, Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
        public string? Bairro { get; set; }

        [FormField(Name = "Complemento", Order = 30, Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.TextArea)]
        public string? Complemento { get; set; }

        [FormField(Name = "Observações", Order = 40, Section = "Status", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Informações adicionais sobre o cliente...", GridColumns = 1)]
        public string? Observacoes { get; set; }

        [FormField(Order = 50, Name = "Foto do Cliente", Section = "Teste", Icon = "fas fa-image", Type = EnumFieldType.Image, ImageSize = "75X75", AllowedExtensions = "jpg,jpeg,png", MaxSizeMB = 5)]
        public string? FotoCliente { get; set; }

        [FormField(Order = 50, Name = "Documento RG (PDF)", Section = "Teste", Icon = "fas fa-file-pdf", Type = EnumFieldType.File, AllowedExtensions = "pdf", MaxSizeMB = 5)]
        public string? DocumentoRG { get; set; }

        // Navigation properties
        public virtual ICollection<Veiculo> Veiculos { get; set; } = [];
    }
}