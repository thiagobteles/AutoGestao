using AutoGestao.Services.Interface;
using System.Diagnostics;

namespace AutoGestao.Middleware
{
    public class AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<AuditMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            // Ignorar rotas específicas
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var ignorePaths = new[] { "/css", "/js", "/images", "/favicon", "/api/audit", "/audit" };

            if (ignorePaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var sucesso = true;
            string? mensagemErro = null;

            try
            {
                await _next(context);

                // Considerar erro se status code >= 400
                sucesso = context.Response.StatusCode < 400;
                if (!sucesso)
                {
                    mensagemErro = $"HTTP {context.Response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                sucesso = false;
                mensagemErro = ex.Message;
                throw;
            }
            finally
            {
                stopwatch.Stop();

                try
                {
                    // Log apenas para usuários autenticados e métodos específicos
                    if (context.User.Identity?.IsAuthenticated == true &&
                        (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE"))
                    {
                        await auditService.LogHttpRequestAsync(
                            context.Request.Path + context.Request.QueryString,
                            context.Request.Method,
                            sucesso,
                            stopwatch.ElapsedMilliseconds,
                            mensagemErro
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao registrar auditoria HTTP");
                }
            }
        }
    }
}