namespace FGT.Models
{
    public class DespesaCreateViewModel
    {
        public int IdFornecedor { get; set; }
        public string Descricao { get; set; } = "";
        public decimal Valor { get; set; }
        public DateTime DataDespesa { get; set; } = DateTime.Today;
    }
}
