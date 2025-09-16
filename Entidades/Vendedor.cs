namespace AutoGestao.Entidades
{
    public class Vendedor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public decimal? PercentualComissao { get; set; }
        public decimal? Meta { get; set; }
        public string Status { get; set; } = "Ativo"; // Ativo, Inativo
        public DateTime DataCadastro { get; set; }

        // Navigation properties
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
        public virtual ICollection<Tarefa> Tarefas { get; set; } = [];
    }
}