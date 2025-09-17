namespace AutoGestao.Entidades
{
    public class Vendedor : BaseEntidade
    {
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public decimal? PercentualComissao { get; set; }
        public decimal? Meta { get; set; }
        public bool Ativo { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
        public virtual ICollection<Tarefa> Tarefas { get; set; } = [];
    }
}