using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Despesa : BaseEntidade
    {
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public string? NumeroNF { get; set; }
        public DateTime DataDespesa { get; set; }
        public EnumStatusDespesa Status { get; set; } = EnumStatusDespesa.Pendente;

        // Foreign Keys
        public int VeiculoId { get; set; }
        public int DespesaTipoId { get; set; }
        public int FornecedorId { get; set; }

        // Navigation properties
        public virtual Veiculo? Veiculo { get; set; }
        public virtual DespesaTipo? DespesaTipo { get; set; }
        public virtual Fornecedor? Fornecedor { get; set; }
    }
}