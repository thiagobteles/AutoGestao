using AutoGestao.Attributes;

namespace AutoGestao.Entidades.Veiculos
{
    public class VeiculoCor : BaseEntidadeEmpresa
    {
        [ReferenceSearchable]
        [ReferenceText]
        public string Descricao { get; set; } = string.Empty;
    }
}