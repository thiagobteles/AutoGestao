using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Modelo de Veículo", Subtitle = "Gerencie os modelos de veículos", Icon = "fas fa-car-side", EnableAjaxSubmit = true)]
    public class VeiculoMarcaModelo : BaseEntidadeEmpresa
    {
        [GridMain("Modelo")]
        [ReferenceSearchable]
        [ReferenceText]
        [FormField(Order = 1, Name = "Descrição do Modelo", Section = "Dados Básicos", Icon = "fas fa-car-side", Type = EnumFieldType.Text, Required = true, GridColumns = 2, Placeholder = "Ex: Uno, Gol, Onix...")]
        public string Descricao { get; set; } = string.Empty;

        [GridField("Marca", Order = 20, Width = "150px")]
        [ReferenceSubtitle(Order = 0, NavigationPath = "VeiculoMarca.Descricao")]
        [FormField(Order = 10, Name = "Marca", Section = "Vínculo", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarca), Required = true, GridColumns = 2)]
        public long? IdVeiculoMarca { get; set; }

        // Navigation properties
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
    }
}