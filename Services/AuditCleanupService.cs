using AutoGestao.Data;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Services
{
    public class AuditCleanupService(ApplicationDbContext context, ILogger<AuditCleanupService> logger) : IAuditCleanupService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<AuditCleanupService> _logger = logger;

        public async Task CleanupOldLogsAsync(int daysToKeep = 365)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

                var logsToDelete = await _context.AuditLogs
                    .Where(log => log.DataHora < cutoffDate)
                    .ToListAsync();

                if (logsToDelete.Count != 0)
                {
                    _context.AuditLogs.RemoveRange(logsToDelete);
                    var deletedCount = await _context.SaveChangesAsync();

                    _logger.LogInformation($"Cleanup de auditoria: {deletedCount} logs removidos (mais antigos que {daysToKeep} dias)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante cleanup de logs de auditoria");
            }
        }

        public async Task ArchiveOldLogsAsync(int daysToArchive = 90)
        {
            // Implementar arquivamento em arquivo/sistema externo se necessário
            await Task.CompletedTask;
        }

        public async Task CompressOldLogsAsync(int daysToCompress = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToCompress);

                // Comprimir dados JSON muito grandes
                var logsToCompress = await _context.AuditLogs
                    .Where(log => log.DataHora < cutoffDate)
                    .Where(log => log.ValoresAntigos.Length > 1000 || log.ValoresNovos.Length > 1000)
                    .ToListAsync();

                foreach (var log in logsToCompress)
                {
                    // Simplificar JSON mantendo apenas campos essenciais
                    if (!string.IsNullOrEmpty(log.ValoresAntigos) && log.ValoresAntigos.Length > 1000)
                    {
                        log.ValoresAntigos = "[DADOS COMPRIMIDOS]";
                    }

                    if (!string.IsNullOrEmpty(log.ValoresNovos) && log.ValoresNovos.Length > 1000)
                    {
                        log.ValoresNovos = "[DADOS COMPRIMIDOS]";
                    }
                }

                if (logsToCompress.Count != 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Compressão de auditoria: {logsToCompress.Count} logs comprimidos");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante compressão de logs de auditoria");
            }
        }
    }
}