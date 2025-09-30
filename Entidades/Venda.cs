using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Venda : BaseEntidadeEmpresa
    {
        public decimal ValorVenda { get; set; }
        public decimal? ValorEntrada { get; set; }
        public int? NumeroParcelas { get; set; }
        public EnumFormaPagamento FormaPagamento { get; set; }
        public EnumStatusVenda Status { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataVenda { get; set; }

        // Foreign Keys
        public long IdCliente { get; set; }
        public long IdVeiculo { get; set; }
        public long IdVendedor { get; set; }

        // Navigation properties
        public virtual Cliente Cliente { get; set; }
        public virtual Veiculo Veiculo { get; set; }
        public virtual Vendedor Vendedor { get; set; }
        public virtual ICollection<Parcela> Parcelas { get; set; } = [];
    }
}