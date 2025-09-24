using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Models.Auth
{
    public class DashboardAuditViewModel
    {
        public List<(EnumTipoOperacaoAuditoria Operacao, int Total)> EstatisticasPorOperacao { get; set; } = new();
        public List<(DateTime Data, int Total)> OperacoesPorDia { get; set; } = new();
        public List<(string Usuario, int Total)> UsuariosMaisAtivos { get; set; } = new();
        public List<(string Entidade, int Total)> EntidadesMaisAuditadas { get; set; } = new();
        public int TotalOperacoes { get; set; }
        public int UltimosDias { get; set; }
    }
}