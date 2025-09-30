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
                var (Id, Nome, Email) = GetCurrentUser();

                var auditLog = new AuditLog
                {
                    UsuarioId = Id,
                    UsuarioNome = Nome,
                    UsuarioEmail = Email,
                    EntidadeNome = entidadeNome,
                    EntidadeDisplayName = GetEntityDisplayName(entidadeNome),
                    EntidadeId = entidadeId,
                    TipoOperacao = tipoOperacao,
                    TabelaNome = GetTableName(entidadeNome),
                    ValoresAntigos = valoresAntigos != null ? JsonConvert.SerializeObject(valoresAntigos, Formatting.Indented) : null,
                    ValoresNovos = valoresNovos != null ? JsonConvert.SerializeObject(valoresNovos, Formatting.Indented) : null,
                    CamposAlterados = camposAlterados != null ? string.Join(", ", camposAlterados) : null,
                    IpCliente = GetClientIpAddress(),
                    UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
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
                _logger.LogError(ex, "Erro ao registrar log de auditoria para {EntidadeNome}:{EntidadeId}", entidadeNome, entidadeId);
            }
        }

        public async Task LogLoginAsync(long usuarioId, string usuarioNome, string usuarioEmail, bool sucesso, string? mensagemErro = null)
        {
            try
            {
                var tipoOperacao = sucesso ? EnumTipoOperacaoAuditoria.Login : EnumTipoOperacaoAuditoria.LoginFailed;

                var auditLog = new AuditLog
                {
                    UsuarioId = usuarioId,
                    UsuarioNome = usuarioNome,
                    UsuarioEmail = usuarioEmail,
                    EntidadeNome = "Usuario",
                    EntidadeDisplayName = "Usuário",
                    EntidadeId = usuarioId.ToString(),
                    TipoOperacao = tipoOperacao,
                    TabelaNome = "usuarios",
                    IpCliente = GetClientIpAddress(),
                    UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
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

        public async Task LogHttpRequestAsync(string url, string metodo, bool sucesso, long? duracaoMs = null, string? mensagemErro = null)
        {
            try
            {
                var (Id, Nome, Email) = GetCurrentUser();

                var auditLog = new AuditLog
                {
                    UsuarioId = Id,
                    UsuarioNome = Nome,
                    UsuarioEmail = Email,
                    EntidadeNome = "HttpRequest",
                    EntidadeDisplayName = "Requisição HTTP",
                    EntidadeId = Guid.NewGuid().ToString(),
                    TipoOperacao = EnumTipoOperacaoAuditoria.View,
                    IpCliente = GetClientIpAddress(),
                    UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
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

        public async Task<List<AuditLog>> GetLogsAsync(
            long? usuarioId = null,
            string? entidade = null,
            EnumTipoOperacaoAuditoria? tipoOperacao = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            int skip = 0,
            int take = 50)
        {
            var query = _context.AuditLogs.Include(a => a.Usuario).AsQueryable();

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

        private (long? Id, string Nome, string Email) GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var idClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nome = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";
                var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? "";

                if (int.TryParse(idClaim, out int userId))
                {
                    return (userId, nome, email);
                }
            }

            return (null, "Sistema", "sistema@autogestao.com");
        }

        private string? GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            return ipAddress;
        }

        private string GetEntityDisplayName(string entidadeNome)
        {
            return entidadeNome switch
            {
                "Usuario" => "Usuário",
                "Veiculo" => "Veículo",
                "Cliente" => "Cliente",
                "Vendedor" => "Vendedor",
                "Fornecedor" => "Fornecedor",
                "Produto" => "Produto",
                "Venda" => "Venda",
                "Despesa" => "Despesa",
                _ => entidadeNome
            };
        }

        private string GetTableName(string entidadeNome)
        {
            return entidadeNome.ToLower() + "s";
        }
    }
}
