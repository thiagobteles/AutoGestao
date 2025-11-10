using FGT.Entidades;
using FGT.Entidades.Base;

namespace FGT.Models
{
    public class DashboardViewModel
    {
        // Primeira linha de cards
        public int TotalEmpresasClientes { get; set; }
        public int NotasFiscaisEmitidas { get; set; }
        public int ObrigacoesPendentes { get; set; }
        public int CertificadosVencendo { get; set; }

        // Segunda linha
        public decimal FaturamentoMes { get; set; }
        public decimal FaturamentoNFe { get; set; }
        public decimal FaturamentoNFSe { get; set; }
        public List<ObrigacaoFiscal> ObrigacoesProximas { get; set; } = [];

        // Terceira linha de cards
        public int LancamentosContabeisMes { get; set; }
        public int TotalPlanoContas { get; set; }
        public int AliquotasConfiguradas { get; set; }
        public int TotalContadores { get; set; }

        // Dados adicionais
        public int TotalClientes { get; set; }
        public int VendasMes { get; set; }
        public decimal ValorVendasMes { get; set; }
        public decimal MetaMensal { get; set; } = 150000;
        public decimal ComparativoComMesAnterior { get; set; }
        public List<Cliente> Aniversariantes { get; set; } = [];
        public List<long> UltimasVendas { get; set; } = [];

        // Métricas detalhadas
        public int EmpresasSimples { get; set; }
        public int EmpresasLucroPresumido { get; set; }
        public int EmpresasLucroReal { get; set; }
        public int NotasEmitidas { get; set; }
        public int NotasCanceladas { get; set; }
    }
}