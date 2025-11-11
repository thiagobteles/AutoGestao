using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Marca de Veículo", Subtitle = "Gerencie as marcas de veículos", Icon = "fas fa-car")]
    public class VeiculoMarca : BaseEntidade
    {
        [ReferenceText] // Campo que será utilizado como referencia.
        [GridMain("Descrição")]
        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-car", Type = EnumFieldType.Text, Required = true, GridColumns = 1, Placeholder = "Ex: Fiat, Volkswagen, Chevrolet...")]
        public string Descricao { get; set; } = string.Empty;

        public ICollection<VeiculoMarcaModelo> Modelos { get; set; } = [];
    }
}