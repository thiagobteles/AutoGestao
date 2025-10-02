using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Localização", Subtitle = "Gerencie as localizações de veículos", Icon = "fas fa-map-marker-alt", EnableAjaxSubmit = true)]
    public class VeiculoLocalizacao : BaseEntidadeEmpresa
    {
        [GridMain("Descrição")]
        [ReferenceSearchable]
        [ReferenceText]
        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-map-marker-alt", Type = EnumFieldType.Text, Required = true, GridColumns = 2, Placeholder = "Ex: Pátio Principal, Estacionamento 1...")]
        public string Descricao { get; set; } = string.Empty;
    }
}