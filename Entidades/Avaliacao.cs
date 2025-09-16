namespace AutoGestao.Entidades
{
    public class Avaliacao
    {
        public int Id { get; set; }
        public string MarcaVeiculo { get; set; } = string.Empty;
        public string ModeloVeiculo { get; set; } = string.Empty;
        public int AnoVeiculo { get; set; }
        public string? PlacaVeiculo { get; set; }
        public decimal? ValorOferecido { get; set; }
        public string StatusAvaliacao { get; set; } = "Pendente"; // Pendente, Aprovada, Rejeitada
        public string? Observacoes { get; set; }
        public DateTime DataAvaliacao { get; set; }

        // Foreign Keys
        public int? ClienteId { get; set; }
        public int? VendedorResponsavelId { get; set; }

        // Navigation properties
        public virtual Cliente? Cliente { get; set; }
        public virtual Vendedor? VendedorResponsavel { get; set; }
    }
}