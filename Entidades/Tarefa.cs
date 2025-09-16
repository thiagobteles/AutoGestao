namespace AutoGestao.Entidades
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string Status { get; set; } = "Pendente"; // Pendente, EmAndamento, Concluida, Cancelada
        public string Prioridade { get; set; } = "Media"; // Baixa, Media, Alta, Critica
        public DateTime DataCriacao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public DateTime? DataConclusao { get; set; }

        // Foreign Keys
        public int? ResponsavelId { get; set; }

        // Navigation properties
        public virtual Vendedor? Responsavel { get; set; }
    }
}