using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AutoGestao.Data
{
    public class AuditInterceptor(IAuditService auditService) : SaveChangesInterceptor
    {
        private readonly IAuditService _auditService = auditService;
        private bool _isProcessingAudit = false; // Flag para evitar recursão

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            // Evitar recursão quando estamos salvando logs de auditoria
            if (_isProcessingAudit || eventData.Context == null)
            {
                return result;
            }

            try
            {
                _isProcessingAudit = true;

                var entries = eventData.Context.ChangeTracker.Entries()
                    .Where(e => !(e.Entity is AuditLog) && // Não auditar os próprios logs
                               e.State != EntityState.Unchanged)
                    .ToList();

                foreach (var entry in entries)
                {
                    await ProcessEntityAsync(entry, eventData.Context as ApplicationDbContext, cancellationToken);
                }

                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            finally
            {
                _isProcessingAudit = false;
            }
        }

        private async Task ProcessEntityAsync(EntityEntry entry, ApplicationDbContext context, CancellationToken cancellationToken)
        {
            try
            {
                // Criar uma nova instância do contexto para salvar auditoria
                // Isso evita conflitos com o contexto principal
                using var auditContext = CreateAuditContext(context);

                var auditLog = new AuditLog
                {
                    EntidadeNome = entry.Entity.GetType().Name,
                    EntidadeId = GetEntityId(entry),
                    TipoOperacao = GetActionType(entry.State),
                    Usuario = await GetCurrentUserAsync(auditContext),
                    DataHora = DateTime.UtcNow,
                    ValoresAntigos = GetOldValues(entry),
                    ValoresNovos = GetNewValues(entry)
                };

                auditContext.AuditLogs.Add(auditLog);
                await auditContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log do erro sem interromper o processo principal
                Console.WriteLine($"Erro na auditoria: {ex.Message}");
            }
        }

        private ApplicationDbContext CreateAuditContext(ApplicationDbContext originalContext)
        {
            // Criar uma nova instância do contexto apenas para auditoria
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(originalContext.Database.GetConnectionString());
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private string GetEntityId(EntityEntry entry)
        {
            var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            return idProperty?.CurrentValue?.ToString() ?? "Unknown";
        }

        private EnumTipoOperacaoAuditoria GetActionType(EntityState state)
        {
            return state switch
            {
                EntityState.Added => EnumTipoOperacaoAuditoria.Create,
                EntityState.Modified => EnumTipoOperacaoAuditoria.Update,
                EntityState.Deleted => EnumTipoOperacaoAuditoria.Delete,
                _ => EnumTipoOperacaoAuditoria.Unknown
            };
        }

        private async Task<Usuario> GetCurrentUserAsync(ApplicationDbContext auditContext)
        {
            // Implementar lógica para obter usuário atual
            return await auditContext.Usuarios.FirstOrDefaultAsync();
        }

        private string GetOldValues(EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
            {
                return "{}";
            }

            var oldValues = new Dictionary<string, object>();
            foreach (var property in entry.Properties)
            {
                if (property.OriginalValue != null)
                {
                    oldValues[property.Metadata.Name] = property.OriginalValue;
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(oldValues);
        }

        private string GetNewValues(EntityEntry entry)
        {
            if (entry.State == EntityState.Deleted)
            {
                return "{}";
            }

            var newValues = new Dictionary<string, object>();
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue != null)
                {
                    newValues[property.Metadata.Name] = property.CurrentValue;
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(newValues);
        }
    }
}