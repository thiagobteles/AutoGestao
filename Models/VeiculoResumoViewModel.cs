using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Models
{
    public class VeiculoResumoViewModel
    {
        public Veiculo Veiculo { get; set; }
        public decimal TotalDespesas { get; set; }
        public int QtdDocumentos { get; set; }
        public int QtdFotos { get; set; }
        public List<Despesa> UltimasDespesas { get; set; } = [];
    }
}
