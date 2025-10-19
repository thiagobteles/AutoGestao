namespace AutoGestao.Middleware
{
    /// <summary>
    /// Middleware para debugging do sistema ultra-genérico
    /// </summary>
    public class UltraGenericDebugMiddleware(RequestDelegate next, ILogger<UltraGenericDebugMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<UltraGenericDebugMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            // Debug de requests para controllers ultra-genéricos
            if (context.Request.Path.Value?.Contains("EntityConfig") == true)
            {
                _logger.LogInformation("🔍 Debug request: {Path}", context.Request.Path);
            }

            await _next(context);
        }
    }
}
