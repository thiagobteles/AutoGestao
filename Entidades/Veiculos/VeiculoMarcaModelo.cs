namespace AutoGestao.Entidades.Veiculos
{
    public class VeiculoMarcaModelo : BaseEntidadeEmpresa
    {
        public string Descricao { get; set; } = string.Empty;

        // Foreign Keys
        public long? IdVeiculoMarca { get; set; }

        // Navigation properties
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
    }
}