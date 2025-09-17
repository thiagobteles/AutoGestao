namespace AutoGestao.Entidades.Veiculos
{
    public class VeiculoMarcaModelo : BaseEntidade
    {
        public string Descricao { get; set; } = string.Empty;

        // Foreign Keys
        public int? VeiculoMarcaId { get; set; }

        // Navigation properties
        public virtual VeiculoMarca? Marca { get; set; }
    }
}