using AutoGestao.Services.Interface;

public class AuditCleanupBackgroundService(IServiceProvider serviceProvider, ILogger<AuditCleanupBackgroundService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<AuditCleanupBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var cleanupService = scope.ServiceProvider.GetRequiredService<IAuditCleanupService>();

                // Executar limpeza diariamente às 2:00 AM
                var now = DateTime.Now;
                var nextRun = now.Date.AddDays(1).AddHours(2);
                var delay = nextRun - now;

                await Task.Delay(delay, stoppingToken);

                await cleanupService.CompressOldLogsAsync(30);
                await cleanupService.CleanupOldLogsAsync(365);

                _logger.LogInformation("Limpeza automática de auditoria executada com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na limpeza automática de auditoria");
            }
        }
    }
}