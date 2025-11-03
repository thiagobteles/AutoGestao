using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Modelo de Veículo", Subtitle = "Gerencie os modelos de veículos", Icon = "fas fa-car-side", EnableAjaxSubmit = true)]
    public class VeiculoMarcaModelo : BaseEntidade
    {
        [ReferenceText] // Campo que será utilizado como referencia.
        [GridMain("Descrição")] // Grid Item já padronizado.
        [FormField(Order = 1, Name = "Descrição do Modelo", Section = "Dados Básicos", Icon = "fas fa-car-side", Type = EnumFieldType.Text, Required = true, GridColumns = 2, Placeholder = "Ex: Uno, Gol, Onix...")]
        public string Descricao { get; set; } = string.Empty;

        // Definição de como o campo aparece no fluxo de Cadastro e edição.
        [FormField(Order = 10, Name = "Marca", Section = "Vínculo", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarca), Required = true, GridColumns = 2)]
        [ReferenceSubtitle(Order = 0, NavigationPath = "VeiculoMarca.Descricao")] // Quando o campo for referênciado irá mostrar esse campo como subtitulo.
        [GridField("Marca", Order = 20, Width = "150px")] // Definição de como o campo aparece na grid
        public long? IdVeiculoMarca { get; set; }

        public virtual VeiculoMarca? VeiculoMarca { get; set; }
    }
}