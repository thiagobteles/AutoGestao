using AutoGestao.Attributes;

namespace AutoGestao.Entidades.Veiculos
{
    public class VeiculoMarcaModelo : BaseEntidadeEmpresa
    {
        [ReferenceSearchable]
        [ReferenceText]
        public string Descricao { get; set; } = string.Empty;

        // O Subtitle virá da navegação para VeiculoMarca.Descricao
        [ReferenceSubtitle(Order = 0, NavigationPath = "VeiculoMarca.Descricao")]
        public long? IdVeiculoMarca { get; set; }

        // Navigation properties
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
    }
}