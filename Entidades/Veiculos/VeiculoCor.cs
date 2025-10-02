using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Cor de Veículo", Subtitle = "Gerencie as cores disponíveis", Icon = "fas fa-palette", EnableAjaxSubmit = true)]
    public class VeiculoCor : BaseEntidadeEmpresa
    {
        [GridMain("Descrição")]
        [ReferenceSearchable]
        [ReferenceText]
        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-palette", Type = EnumFieldType.Text, Required = true, GridColumns = 2, Placeholder = "Ex: Branco, Preto, Prata...")]
        public string Descricao { get; set; } = string.Empty;
    }
}