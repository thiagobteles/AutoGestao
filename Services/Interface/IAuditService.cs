using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Services.Interface
{
    public interface IAuditService
    {
        Task LogAsync(
            string entidadeNome,
            string entidadeId,
            EnumTipoOperacaoAuditoria tipoOperacao,
            object? valoresAntigos = null,
            object? valoresNovos = null,
            string[]? camposAlterados = null,
            string? mensagemErro = null);

        Task LogLoginAsync(long usuarioId, long? IdEmpresa, bool sucesso, string? mensagemErro = null);

        Task LogLogoutAsync(long usuarioId, long? IdEmpresa);

        Task LogHttpRequestAsync(string url, string metodo, bool sucesso, long? duracaoMs = null, string? mensagemErro = null);

        Task LogViewAsync(string entidadeNome, string entidadeId, string? mensagemErro = null);

        Task LogPrintReportAsync(string reportNome, string? entidadeId = null, bool sucesso = true, string? mensagemErro = null);

        Task LogActionAsync(string actionNome, string entidadeNome, string entidadeId, bool sucesso = true, string? mensagemErro = null, object? dadosAcao = null);

        Task<List<AuditLog>> GetLogsAsync(
            long? usuarioId = null,
            string? entidade = null,
            EnumTipoOperacaoAuditoria? tipoOperacao = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            int skip = 0,
            int take = 50);

        Task<int> GetLogsCountAsync(
            long? usuarioId = null,
            string? entidade = null,
            EnumTipoOperacaoAuditoria? tipoOperacao = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);
    }
}