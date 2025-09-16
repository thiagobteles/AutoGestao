namespace AutoGestao.Entidades
{
    public class Despesa
    {
        public int Id { get; set; }
        public string TipoDespesa { get; set; } = string.Empty; // Revisao, Reparo, Documentacao, etc.
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string? Fornecedor { get; set; }
        public string? NumeroNF { get; set; }
        public DateTime DataDespesa { get; set; }
        public string Status { get; set; } = "Pendente"; // Pendente, Pago, Cancelado

        // Foreign Keys
        public int? VeiculoId { get; set; }

        // Navigation properties
        public virtual Veiculo? Veiculo { get; set; }
    }
}