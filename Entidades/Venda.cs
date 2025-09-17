using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Venda : BaseEntidade
    {
        public required decimal ValorVenda { get; set; }
        public decimal? ValorEntrada { get; set; }
        public int? NumeroParcelas { get; set; }
        public EnumFormaPagamento? FormaPagamento { get; set; }
        public EnumStatusVenda Status { get; set; } = EnumStatusVenda.Pendente;
        public string? Observacoes { get; set; }
        public required DateTime DataVenda { get; set; }

        // Foreign Keys
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public int VendedorId { get; set; }

        // Navigation properties
        public virtual required Cliente Cliente { get; set; }
        public virtual required Veiculo Veiculo { get; set; }
        public virtual required Vendedor Vendedor { get; set; }
        public virtual ICollection<Parcela> Parcelas { get; set; } = [];
    }
}