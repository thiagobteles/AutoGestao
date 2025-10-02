using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Foto do Veículo", Subtitle = "Gerencie as fotos dos veículos", Icon = "fas fa-image", EnableAjaxSubmit = true)]
    public class VeiculoFoto : BaseEntidadeEmpresa
    {
        [GridMain("Nome do Arquivo")]
        [FormField(Order = 1, Name = "Nome do Arquivo", Section = "Arquivo", Icon = "fas fa-file", Type = EnumFieldType.Text, Required = true, ReadOnly = true, GridColumns = 2)]
        public string NomeArquivo { get; set; } = string.Empty;

        [FormField(Order = 1, Name = "Caminho do Arquivo", Section = "Arquivo", Icon = "fas fa-folder", Type = EnumFieldType.Text, Required = true, ReadOnly = true)]
        public string CaminhoArquivo { get; set; } = string.Empty;

        [GridField("Descrição", Order = 20)]
        [FormField(Order = 10, Name = "Descrição", Section = "Informações", Icon = "fas fa-align-left", Type = EnumFieldType.Text, GridColumns = 2, Placeholder = "Descrição da foto...")]
        public string? Descricao { get; set; }

        [GridField("Data Upload", Order = 30, Width = "110px")]
        [FormField(Order = 20, Name = "Data do Upload", Section = "Informações", Icon = "fas fa-calendar", Type = EnumFieldType.DateTime, Required = true, ReadOnly = true, GridColumns = 3)]
        public DateTime DataUpload { get; set; }

        [GridField("Principal", Order = 35, Width = "80px")]
        [FormField(Order = 30, Name = "Foto Principal", Section = "Destaque", Icon = "fas fa-star", Type = EnumFieldType.Checkbox, GridColumns = 3)]
        public bool Principal { get; set; }

        [FormField(Order = 40, Name = "Veículo", Section = "Vínculo", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(Veiculo), Required = true, ReadOnly = true)]
        public long IdVeiculo { get; set; }

        // Navigation properties
        public virtual Veiculo Veiculo { get; set; } = null!;
    }
}