namespace AutoGestao.Entidades
{
    public class Venda
    {
        public int Id { get; set; }
        public decimal ValorVenda { get; set; }
        public decimal? ValorEntrada { get; set; }
        public int? NumeroParcelas { get; set; }
        public string? FormaPagamento { get; set; }
        public string Status { get; set; } = "Pendente"; // Pendente, Concluida, Cancelada
        public string? Observacoes { get; set; }
        public DateTime DataVenda { get; set; }

        // Foreign Keys
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public int? VendedorId { get; set; }

        // Navigation properties
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Veiculo Veiculo { get; set; } = null!;
        public virtual Vendedor? Vendedor { get; set; }
        public virtual ICollection<Parcela> Parcelas { get; set; } = [];
    }
}