namespace AutoGestao.Entidades
{
    public class Parcela
    {
        public int Id { get; set; }
        public int NumeroParcela { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public decimal? ValorPago { get; set; }
        public string Status { get; set; } = "Pendente"; // Pendente, Paga, Vencida

        // Foreign Keys
        public int VendaId { get; set; }

        // Navigation properties
        public virtual Venda Venda { get; set; } = null!;
    }
}