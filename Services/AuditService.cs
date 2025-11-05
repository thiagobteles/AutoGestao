using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AutoGestao.Services
{
    public class AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<AuditService> logger) : IAuditService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<AuditService> _logger = logger;

        public async Task LogAsync(
            string entidadeNome,
            string entidadeId,
            EnumTipoOperacaoAuditoria tipoOperacao,
            object? valoresAntigos = null,
            object? valoresNovos = null,
            string[]? camposAlterados = null,
            string? mensagemErro = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var usuario = GetCurrentUser();

                // ⚠️ CORREÇÃO PRINCIPAL: Validar IdEmpresa
                var idEmpresaValido = await ValidarEObterIdEmpresaAsync(usuario.IdEmpresa);

                // Se não houver empresa válida, logar o erro mas não falhar
                if (!idEmpresaValido.HasValue)
                {
                    _logger.LogWarning(
                        "Tentativa de criar audit log sem empresa válida. Usuário: {UsuarioId}, Empresa: {IdEmpresa}",
                        usuario.Id,
                        usuario.IdEmpresa
                    );
                    return; // Não cria o log se não houver empresa válida
                }

                var auditLog = new AuditLog
                {
                    IdEmpresa = idEmpresaValido.Value,
                    UsuarioId = usuario.Id,
                    EntidadeNome = entidadeNome,
                    EntidadeId = entidadeId,
                    TipoOperacao = tipoOperacao,
                    TabelaNome = GetTableName(entidadeNome),
                    ValoresAntigos = valoresAntigos != null ? JsonConvert.SerializeObject(valoresAntigos, Formatting.Indented) : null,
                    ValoresNovos = valoresNovos != null ? JsonConvert.SerializeObject(valoresNovos, Formatting.Indented) : null,
                    CamposAlterados = camposAlterados != null ? string.Join(", ", camposAlterados) : null,
                    IpCliente = GetClientIpAddress(),
                    UrlRequisicao = httpContext?.Request.Path + httpContext?.Request.QueryString,
                    MetodoHttp = httpContext?.Request.Method,
                    Sucesso = string.IsNullOrEmpty(mensagemErro),
                    MensagemErro = mensagemErro,
                    DataHora = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao registrar log de auditoria para {EntidadeNome}:{EntidadeId}. Erro: {Message}",
                    entidadeNome,
                    entidadeId,
                    ex.Message
                );
            }
        }

        public async Task LogHttpRequestAsync(string url, string metodo, bool sucesso, long? duracaoMs = null, string? mensagemErro = null)
        {
            try
            {
                var usuario = GetCurrentUser();

                // ⚠️ CORREÇÃO: Validar IdEmpresa
                var idEmpresaValido = await ValidarEObterIdEmpresaAsync(usuario.IdEmpresa);

                if (!idEmpresaValido.HasValue)
                {
                    _logger.LogWarning(
                        "Tentativa de criar audit log HTTP sem empresa válida. Usuário: {UsuarioId}, Empresa: {IdEmpresa}",
                        usuario.Id,
                        usuario.IdEmpresa
                    );
                    return;
                }

                var auditLog = new AuditLog
                {
                    IdEmpresa = idEmpresaValido.Value,
                    UsuarioId = usuario.Id,
                    EntidadeNome = "HttpRequest",
                    EntidadeId = Guid.NewGuid().ToString(),
                    TipoOperacao = EnumTipoOperacaoAuditoria.View,
                    IpCliente = GetClientIpAddress(),
                    UrlRequisicao = url,
                    MetodoHttp = metodo,
                    Sucesso = sucesso,
                    MensagemErro = mensagemErro,
                    DuracaoMs = duracaoMs,
                    DataHora = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log de requisição HTTP: {Url}", url);
            }
        }

        public async Task LogLoginAsync(long usuarioId, long? IdEmpresa, bool sucesso, string? mensagemErro = null)
        {
            try
            {
                // Validar IdEmpresa
                var idEmpresaValido = await ValidarEObterIdEmpresaAsync(IdEmpresa);

                if (!idEmpresaValido.HasValue)
                {
                    _logger.LogWarning(
                        "Tentativa de criar audit log de login sem empresa válida. Usuário: {UsuarioId}, Empresa: {IdEmpresa}",
                        usuarioId,
                        IdEmpresa
                    );
                    return;
                }

                var tipoOperacao = sucesso ? EnumTipoOperacaoAuditoria.Login : EnumTipoOperacaoAuditoria.LoginFailed;

                var auditLog = new AuditLog
                {
                    IdEmpresa = idEmpresaValido.Value,
                    UsuarioId = usuarioId,
                    EntidadeNome = "Usuario",
                    EntidadeId = usuarioId.ToString(),
                    TipoOperacao = tipoOperacao,
                    TabelaNome = nameof(Usuario),
                    IpCliente = GetClientIpAddress(),
                    UrlRequisicao = _httpContextAccessor.HttpContext?.Request.Path,
                    MetodoHttp = _httpContextAccessor.HttpContext?.Request.Method,
                    Sucesso = sucesso,
                    MensagemErro = mensagemErro,
                    DataHora = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log de login para usuário {UsuarioId}", usuarioId);
            }
        }

        public async Task LogLogoutAsync(long usuarioId, long? IdEmpresa)
        {
            try
            {
                // Validar IdEmpresa
                var idEmpresaValido = await ValidarEObterIdEmpresaAsync(IdEmpresa);

                if (!idEmpresaValido.HasValue)
                {
                    _logger.LogWarning(
                        "Tentativa de criar audit log de logout sem empresa válida. Usuário: {UsuarioId}, Empresa: {IdEmpresa}",
                        usuarioId,
                        IdEmpresa
                    );
                    return;
                }

                var auditLog = new AuditLog
                {
                    IdEmpresa = idEmpresaValido.Value,
                    UsuarioId = usuarioId,
                    EntidadeNome = "Usuario",
                    EntidadeId = usuarioId.ToString(),
                    TipoOperacao = EnumTipoOperacaoAuditoria.Logout,
                    TabelaNome = nameof(Usuario),
                    IpCliente = GetClientIpAddress(),
                    UrlRequisicao = _httpContextAccessor.HttpContext?.Request.Path,
                    MetodoHttp = _httpContextAccessor.HttpContext?.Request.Method,
                    Sucesso = true,
                    DataHora = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log de logout para usuário {UsuarioId}", usuarioId);
            }
        }

        public async Task LogViewAsync(string entidadeNome, string entidadeId, string? mensagemErro = null)
        {
            await LogAsync(
                entidadeNome,
                entidadeId,
                EnumTipoOperacaoAuditoria.View,
                mensagemErro: mensagemErro
            );
        }

        public async Task LogPrintReportAsync(string reportNome, string? entidadeId = null, bool sucesso = true, string? mensagemErro = null)
        {
            try
            {
                var usuario = GetCurrentUser();
                var idEmpresaValido = await ValidarEObterIdEmpresaAsync(usuario.IdEmpresa);

                if (!idEmpresaValido.HasValue)
                {
                    _logger.LogWarning(
                        "Tentativa de criar audit log de impressão sem empresa válida. Usuário: {UsuarioId}",
                        usuario.Id
                    );
                    return;
                }

                var auditLog = new AuditLog
                {
                    IdEmpresa = idEmpresaValido.Value,
                    UsuarioId = usuario.Id,
                    EntidadeNome = "Relatorio",
                    EntidadeId = entidadeId ?? Guid.NewGuid().ToString(),
                    TipoOperacao = EnumTipoOperacaoAuditoria.PrintReport,
                    TabelaNome = "Relatorio",
                    ValoresNovos = reportNome,
                    IpCliente = GetClientIpAddress(),
                    UrlRequisicao = _httpContextAccessor.HttpContext?.Request.Path,
                    MetodoHttp = _httpContextAccessor.HttpContext?.Request.Method,
                    Sucesso = sucesso,
                    MensagemErro = mensagemErro,
                    DataHora = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log de impressão de relatório: {ReportNome}", reportNome);
            }
        }

        public async Task LogActionAsync(string actionNome, string entidadeNome, string entidadeId, bool sucesso = true, string? mensagemErro = null, object? dadosAcao = null)
        {
            try
            {
                var usuario = GetCurrentUser();
                var idEmpresaValido = await ValidarEObterIdEmpresaAsync(usuario.IdEmpresa);

                if (!idEmpresaValido.HasValue)
                {
                    _logger.LogWarning(
                        "Tentativa de criar audit log de ação sem empresa válida. Usuário: {UsuarioId}",
                        usuario.Id
                    );
                    return;
                }

                var auditLog = new AuditLog
                {
                    IdEmpresa = idEmpresaValido.Value,
                    UsuarioId = usuario.Id,
                    EntidadeNome = entidadeNome,
                    EntidadeId = entidadeId,
                    TipoOperacao = EnumTipoOperacaoAuditoria.Action,
                    TabelaNome = entidadeNome.ToLower(),
                    ValoresNovos = dadosAcao != null ? JsonConvert.SerializeObject(new { acao = actionNome, dados = dadosAcao }, Formatting.Indented) : actionNome,
                    IpCliente = GetClientIpAddress(),
                    UrlRequisicao = _httpContextAccessor.HttpContext?.Request.Path,
                    MetodoHttp = _httpContextAccessor.HttpContext?.Request.Method,
                    Sucesso = sucesso,
                    MensagemErro = mensagemErro,
                    DataHora = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log de ação: {ActionNome} para {EntidadeNome}:{EntidadeId}", actionNome, entidadeNome, entidadeId);
            }
        }

        /// <summary>
        /// Valida se o IdEmpresa existe na tabela empresas
        /// </summary>
        private async Task<long?> ValidarEObterIdEmpresaAsync(long? idEmpresa)
        {
            if (!idEmpresa.HasValue || idEmpresa.Value <= 0)
            {
                _logger.LogWarning("IdEmpresa inválido ou não informado: {IdEmpresa}", idEmpresa);
                return null;
            }

            try
            {
                // Verificar se a empresa existe
                var empresaExiste = await _context.Empresas.AnyAsync(e => e.Id == idEmpresa.Value);
                if (!empresaExiste)
                {
                    _logger.LogWarning("Empresa não encontrada no banco de dados. IdEmpresa: {IdEmpresa}", idEmpresa);
                    return null;
                }

                return idEmpresa.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao validar IdEmpresa: {IdEmpresa}. Erro: {Message}",
                    idEmpresa,
                    ex.Message
                );
                return null;
            }
        }

        /// <summary>
        /// Obtém usuário atual do contexto HTTP
        /// </summary>
        private (long Id, string Nome, string Email, long? IdEmpresa) GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var claims = httpContext.User.Claims;

                var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Sistema";
                var userEmail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
                var empresaId = claims.FirstOrDefault(c => c.Type == "IdEmpresa")?.Value;

                return (
                    Id: long.TryParse(userId, out var id) ? id : 0,
                    Nome: userName,
                    Email: userEmail,
                    IdEmpresa: long.TryParse(empresaId, out var empId) ? empId : null
                );
            }

            // Usuário não autenticado ou sistema
            return (0, "Sistema", "sistema@autogestao.com", null);
        }

        private string GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return "Unknown";
            }

            var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            return ip ?? "Unknown";
        }

        private static string GetTableName(string entidadeNome)
        {
            return entidadeNome.ToLower();
        }

        public async Task<List<AuditLog>> GetLogsAsync(
            long? usuarioId = null,
            string? entidade = null,
            EnumTipoOperacaoAuditoria? tipoOperacao = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            int skip = 0,
            int take = 50)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (usuarioId.HasValue)
            {
                query = query.Where(a => a.UsuarioId == usuarioId.Value);
            }

            if (!string.IsNullOrEmpty(entidade))
            {
                query = query.Where(a => a.EntidadeNome == entidade);
            }

            if (tipoOperacao.HasValue)
            {
                query = query.Where(a => a.TipoOperacao == tipoOperacao.Value);
            }

            if (dataInicio.HasValue)
            {
                query = query.Where(a => a.DataHora >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(a => a.DataHora <= dataFim.Value);
            }

            return await query
                .OrderByDescending(a => a.DataHora)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetLogsCountAsync(
            long? usuarioId = null,
            string? entidade = null,
            EnumTipoOperacaoAuditoria? tipoOperacao = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (usuarioId.HasValue)
            {
                query = query.Where(a => a.UsuarioId == usuarioId.Value);
            }

            if (!string.IsNullOrEmpty(entidade))
            {
                query = query.Where(a => a.EntidadeNome.Contains(entidade));
            }

            if (tipoOperacao.HasValue)
            {
                query = query.Where(a => a.TipoOperacao == tipoOperacao.Value);
            }

            if (dataInicio.HasValue)
            {
                query = query.Where(a => a.DataHora >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(a => a.DataHora <= dataFim.Value);
            }

            return await query.CountAsync();
        }
    }
}