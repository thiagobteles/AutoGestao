using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Tarefa : BaseEntidadeEmpresa
    {
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public EnumStatusTarefa Status { get; set; }
        public EnumPrioridade Prioridade { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public DateTime? DataConclusao { get; set; }

        // Foreign Keys
        public int? ResponsavelId { get; set; }
        public int? ResponsavelUsuarioId { get; set; }

        // Navigation properties
        public virtual Vendedor? Responsavel { get; set; }
        public virtual Usuario? ResponsavelUsuario { get; set; }
    }
}