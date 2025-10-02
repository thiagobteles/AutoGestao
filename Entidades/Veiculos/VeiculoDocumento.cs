using AutoGestao.Attributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Documento do Veículo", Subtitle = "Gerencie os documentos dos veículos", Icon = "fas fa-file-alt", EnableAjaxSubmit = true)]
    public class VeiculoDocumento : BaseEntidadeEmpresa
    {
        [GridField("Tipo", Order = 15, Width = "150px")]
        [FormField(Order = 1, Name = "Tipo de Documento", Section = "Identificação", Icon = "fas fa-tags", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public EnumTipoDocumento TipoDocumento { get; set; } = EnumTipoDocumento.Nenhum;

        [GridMain("Nome do Arquivo")]
        [FormField(Order = 10, Name = "Nome do Arquivo", Section = "Arquivo", Icon = "fas fa-file", Type = EnumFieldType.Text, Required = true, ReadOnly = true, GridColumns = 2)]
        public string NomeArquivo { get; set; } = string.Empty;

        [FormField(Order = 10, Name = "Caminho do Arquivo", Section = "Arquivo", Icon = "fas fa-folder", Type = EnumFieldType.Text, Required = true, ReadOnly = true)]
        public string CaminhoArquivo { get; set; } = string.Empty;

        [GridField("Observações", Order = 25)]
        [FormField(Order = 20, Name = "Observações", Section = "Informações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Required = true, Placeholder = "Observações sobre o documento...")]
        public string Observacoes { get; set; } = string.Empty;

        [GridField("Data Upload", Order = 30, Width = "110px")]
        [FormField(Order = 30, Name = "Data do Upload", Section = "Informações", Icon = "fas fa-calendar", Type = EnumFieldType.DateTime, Required = true, ReadOnly = true, GridColumns = 3)]
        public DateTime DataUpload { get; set; }

        [FormField(Order = 40, Name = "Veículo", Section = "Vínculo", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(Veiculo), Required = true, ReadOnly = true)]
        public long IdVeiculo { get; set; }

        // Navigation properties
        public virtual Veiculo Veiculo { get; set; } = null!;
    }
}