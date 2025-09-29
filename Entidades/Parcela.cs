using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Parcela : BaseEntidadeEmpresa
    {
        public int NumeroParcela { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public decimal? ValorPago { get; set; }
        public EnumStatusParcela Status { get; set; } = EnumStatusParcela.Pendente;
        public string? Observacoes {  get; set; }

        // Foreign Keys
        public int VendaId { get; set; }

        // Navigation properties
        public virtual Venda Venda { get; set; }
    }
}