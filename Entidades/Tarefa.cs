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

        // Foreign Keys (manter o ResponsavelId existente por compatibilidade)
        public int? ResponsavelId { get; set; }
        public int? ResponsavelUsuarioId { get; set; } // Nova FK para Usuario

        // Navigation properties
        public virtual Vendedor? Responsavel { get; set; } // Existente
        public virtual Usuario? ResponsavelUsuario { get; set; } // Nova
    }
}