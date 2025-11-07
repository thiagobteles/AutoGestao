using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Filial", Subtitle = "Gerencie as filiais", Icon = "fas fa-building")]
    public class VeiculoFilial : BaseEntidade
    {
        [GridMain("Descrição")]
        [ReferenceSearchable]
        [ReferenceText]
        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Text, Required = true, GridColumns = 1, Placeholder = "Ex: Matriz, Filial Norte, Filial Sul...")]
        public string Descricao { get; set; } = string.Empty;
    }
}