using AutoGestao.Entidades;

namespace AutoGestao.Models
{
    public class DashboardViewModel
    {
        public int TotalVeiculos { get; set; }
        public int VeiculosEstoque { get; set; }
        public int VeiculosVendidos { get; set; }
        public int TotalClientes { get; set; }
        public int VendasMes { get; set; }
        public List<Cliente> Aniversariantes { get; set; }
        public List<Venda> UltimasVendas {  get; set; }
        public decimal ValorVendasMes { get; set; }
    }
}