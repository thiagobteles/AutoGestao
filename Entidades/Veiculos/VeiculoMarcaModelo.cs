using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Modelo de Veículo", Subtitle = "Dados do modelo", GridTitle = "Modelo de Veículo", GridSubTitle = "Gerencie os modelos de veículos", Icon = "fas fa-car-side")]
    public class VeiculoMarcaModelo : BaseEntidade
    {
        // Definição de como o campo aparece no fluxo de Cadastro e edição.
        [FormField(Order = 1, Name = "Marca", Section = "Vínculo", Icon = "fas fa-car", Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarca), Required = true, GridColumns = 1)]
        [ReferenceSubtitle(Order = 0, NavigationPath = "VeiculoMarca.Descricao")] // Quando o campo for referênciado irá mostrar esse campo como subtitulo.
        public long? IdVeiculoMarca { get; set; }

        [GridComposite("Marca", Order = 1, Width = "200px", NavigationPaths = new[] { "VeiculoMarca.Descricao" })]
        public string Marca => $"{VeiculoMarca?.Descricao ?? "N/A"}";

        [ReferenceText] // Campo que será utilizado como referencia.
        [GridMain("Descrição")] // Grid Item já padronizado.
        [FormField(Order = 10, Name = "Descrição do Modelo", Section = "Dados Básicos", Icon = "fas fa-car-side", Type = EnumFieldType.Text, Required = true, GridColumns = 1, Placeholder = "Ex: Uno, Gol, Onix...")]
        public string Descricao { get; set; } = string.Empty;


        public virtual VeiculoMarca? VeiculoMarca { get; set; }
    }
}