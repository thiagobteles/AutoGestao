using AutoGestao.Attributes;

namespace AutoGestao.Entidades.Veiculos
{
    public class VeiculoMarca : BaseEntidadeEmpresa
    {
        [ReferenceSearchable]
        [ReferenceText]
        public string Descricao { get; set; } = string.Empty;

        public ICollection<VeiculoMarcaModelo> Modelos { get; set; } = [];
    }
}