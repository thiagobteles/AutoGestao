using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Documento do Veículo", Subtitle = "Gerencie os documentos dos veículos", Icon = "fas fa-file-alt")]
    public class VeiculoDocumento : BaseEntidade
    {
        [GridField("Tipo", Order = 15, Width = "150px")]
        [FormField(Order = 1, Name = "Tipo de Documento", Section = "Identificação", Icon = "fas fa-tags", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public EnumTipoDocumento TipoDocumento { get; set; } = EnumTipoDocumento.Nenhum;

        [GridMain("Documento")]
        [FormField(Order = 2, Name = "Documento", Section = "Identificação", Icon = "fas fa-image", Type = EnumFieldType.Image, ImageSize = "75X75", AllowedExtensions = "jpg,jpeg,png,pdf", MaxSizeMB = 5)]
        public string? Documento { get; set; }

        [GridField("Observações", Order = 25)]
        [FormField(Order = 3, Name = "Observações", Section = "Identificação", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Required = false, Placeholder = "Observações sobre o documento...")]
        public string Observacoes { get; set; } = string.Empty;

        [GridField("Data Upload", Order = 30, Width = "110px")]
        [FormField(Order = 4, Name = "Data do Upload", Section = "Identificação", Icon = "fas fa-calendar", Type = EnumFieldType.DateTime, Required = false, ReadOnly = false, GridColumns = 3)]
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;

        [FormField(Order = 20, Name = "Veículo", Section = "Vínculo", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(Veiculo), Required = true, ReadOnly = false)]
        public long IdVeiculo { get; set; }

        // Navigation properties
        public virtual Veiculo Veiculo { get; set; } = null!;
    }
}