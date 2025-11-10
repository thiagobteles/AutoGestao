using FGT.Services.Interface;
using System.Diagnostics;

namespace FGT.Middleware
{
    public class AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<AuditMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            // Ignorar rotas específicas de assets e APIs de auditoria
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var ignorePaths = new[] { "/css", "/js", "/images", "/favicon", "/lib", "/fonts", "/api/audit", "/audit" };

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
                    // Log para usuários autenticados
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        // Logar todas as operações: POST, PUT, DELETE e GET (visualização)
                        // Para GET, apenas logar rotas específicas de entidades (não páginas de listagem genéricas)
                        var shouldLog = context.Request.Method == "POST"
                            || context.Request.Method == "PUT"
                            || context.Request.Method == "DELETE"
                            || (context.Request.Method == "GET" && ShouldLogGetRequest(path));

                        if (shouldLog)
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
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao registrar auditoria HTTP");
                }
            }
        }

        /// <summary>
        /// Determina se uma requisição GET deve ser auditada
        /// </summary>
        private static bool ShouldLogGetRequest(string path)
        {
            // Logar apenas requisições GET para rotas de detalhes ou visualização de entidades
            // Exemplos: /Veiculo/Details/5, /Cliente/Edit/3, /ReportBuilder/Edit/1
            var logPatterns = new[]
            {
                "/details/",
                "/edit/",
                "/view/",
                "/visualizar/",
                "/imprimir/",
                "/relatorio/"
            };

            return logPatterns.Any(pattern => path.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }
    }
}