using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Tarefa : BaseEntidade
    {
        public required string Titulo { get; set; }
        public string? Descricao { get; set; }
        public EnumStatusTarefa Status { get; set; } = EnumStatusTarefa.Pendente;
        public EnumPrioridade Prioridade { get; set; } = EnumPrioridade.Media;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public DateTime? DataConclusao { get; set; }

        // Foreign Keys
        public int? ResponsavelId { get; set; }

        // Navigation properties
        public virtual Vendedor? Responsavel { get; set; }
    }
}