namespace AutoGestao.Services.Interface
{
    public interface IAuditCleanupService
    {
        Task CleanupOldLogsAsync(int daysToKeep = 365);
        Task ArchiveOldLogsAsync(int daysToArchive = 90);
        Task CompressOldLogsAsync(int daysToCompress = 30);
    }
}
