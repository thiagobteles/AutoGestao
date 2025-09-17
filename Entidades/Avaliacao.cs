using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Avaliacao : BaseEntidade
    {
        public int AnoVeiculo { get; set; }
        public string? PlacaVeiculo { get; set; }
        public decimal? ValorOferecido { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataAvaliacao { get; set; }
        public EnumStatusAvaliacao StatusAvaliacao { get; set; } = EnumStatusAvaliacao.Pendente;

        // Foreign Keys
        public int? ClienteId { get; set; }
        public int? VendedorResponsavelId { get; set; }
        public int? VeiculoMarcaId { get; set; }
        public int? VeiculoMarcaModeloId { get; set; }

        // Navigation properties
        public virtual Cliente? Cliente { get; set; }
        public virtual Vendedor? VendedorResponsavel { get; set; }
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
        public virtual VeiculoMarcaModelo? VeiculoMarcaModelo { get; set; }
    }
}