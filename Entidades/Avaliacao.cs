using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Avaliacao : BaseEntidadeEmpresa
    {
        public int AnoVeiculo { get; set; }
        public string? PlacaVeiculo { get; set; }
        public decimal? ValorOferecido { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataAvaliacao { get; set; }
        public EnumStatusAvaliacao StatusAvaliacao { get; set; }

        // Foreign Keys
        public long? IdCliente { get; set; }
        public long? IdVendedorResponsavel { get; set; }
        public long? IdVeiculoMarca { get; set; }
        public long? IdVeiculoMarcaModelo { get; set; }

        // Navigation properties
        public virtual Cliente? Cliente { get; set; }
        public virtual Vendedor? VendedorResponsavel { get; set; }
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
        public virtual VeiculoMarcaModelo? VeiculoMarcaModelo { get; set; }
    }
}