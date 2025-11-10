using FGT.Enumerador.Gerais;

namespace FGT.Models.Auth
{
    public class DashboardAuditViewModel
    {
        public List<(EnumTipoOperacaoAuditoria Operacao, int Total)> EstatisticasPorOperacao { get; set; } = [];
        public List<(DateTime Data, int Total)> OperacoesPorDia { get; set; } = [];
        public List<(string Usuario, int Total)> UsuariosMaisAtivos { get; set; } = [];
        public List<(string Entidade, int Total)> EntidadesMaisAuditadas { get; set; } = [];
        public int TotalOperacoes { get; set; }
        public int UltimosDias { get; set; }
    }
}