using AutoGestao.Data;
using AutoGestao.Entidades.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Helpers
{
    public static class AuditHelper
    {
        public static async Task<List<AuditLog>> GetEntityAuditHistory(
            ApplicationDbContext context,
            string entityName,
            string entityId,
            int maxRecords = 50)
        {
            return await context.AuditLogs
                .Where(a => a.EntidadeNome == entityName && a.EntidadeId == entityId)
                .OrderByDescending(a => a.DataHora)
                .Take(maxRecords)
                .ToListAsync();
        }

        public static async Task<int> GetEntityAuditCount(
            ApplicationDbContext context,
            string entityName,
            string entityId)
        {
            return await context.AuditLogs
                .CountAsync(a => a.EntidadeNome == entityName && a.EntidadeId == entityId);
        }

        public static async Task<List<AuditLog>> GetUserAuditHistory(
            ApplicationDbContext context,
            int userId,
            DateTime? since = null,
            int maxRecords = 100)
        {
            var query = context.AuditLogs.Where(a => a.UsuarioId == userId);

            if (since.HasValue)
            {
                query = query.Where(a => a.DataHora >= since.Value);
            }

            return await query
                .OrderByDescending(a => a.DataHora)
                .Take(maxRecords)
                .ToListAsync();
        }
    }
}