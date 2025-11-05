using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace AutoGestao.Data
{
    /// <summary>
    /// Interceptor para auditoria automática de operações CRUD no banco de dados
    /// </summary>
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
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            try
            {
                _isProcessingAudit = true;

                var entries = eventData.Context.ChangeTracker.Entries()
                    .Where(e => !(e.Entity is AuditLog) && // Não auditar os próprios logs
                               e.State != EntityState.Unchanged &&
                               e.State != EntityState.Detached)
                    .ToList();

                // Processar auditoria após salvar as mudanças (para ter IDs gerados)
                var auditTasks = entries.Select(entry => new
                {
                    Entry = entry,
                    State = entry.State,
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = GetEntityId(entry),
                    OldValues = GetOldValues(entry),
                    NewValues = GetNewValues(entry),
                    ModifiedProperties = entry.State == EntityState.Modified
                        ? entry.Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name).ToArray()
                        : null
                }).ToList();

                // Salvar as mudanças primeiro
                var saveResult = await base.SavingChangesAsync(eventData, result, cancellationToken);

                // Agora processar auditoria de forma assíncrona
                _ = Task.Run(async () =>
                {
                    foreach (var audit in auditTasks)
                    {
                        try
                        {
                            await _auditService.LogAsync(
                                audit.EntityName,
                                audit.EntityId,
                                GetActionType(audit.State),
                                valoresAntigos: string.IsNullOrEmpty(audit.OldValues) ? null : JsonSerializer.Deserialize<object>(audit.OldValues),
                                valoresNovos: string.IsNullOrEmpty(audit.NewValues) ? null : JsonSerializer.Deserialize<object>(audit.NewValues),
                                camposAlterados: audit.ModifiedProperties
                            );
                        }
                        catch
                        {
                            // Ignorar erros de auditoria para não afetar a operação principal
                        }
                    }
                }, cancellationToken);

                return saveResult;
            }
            finally
            {
                _isProcessingAudit = false;
            }
        }

        private static string GetEntityId(EntityEntry entry)
        {
            var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            return idProperty?.CurrentValue?.ToString() ?? "0";
        }

        private static EnumTipoOperacaoAuditoria GetActionType(EntityState state)
        {
            return state switch
            {
                EntityState.Added => EnumTipoOperacaoAuditoria.Create,
                EntityState.Modified => EnumTipoOperacaoAuditoria.Update,
                EntityState.Deleted => EnumTipoOperacaoAuditoria.Delete,
                _ => EnumTipoOperacaoAuditoria.Unknown
            };
        }

        private static string GetOldValues(EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
            {
                return string.Empty;
            }

            var oldValues = new Dictionary<string, object?>();
            foreach (var property in entry.Properties.Where(p => p.OriginalValue != null))
            {
                oldValues[property.Metadata.Name] = property.OriginalValue;
            }

            return oldValues.Count > 0 ? JsonSerializer.Serialize(oldValues) : string.Empty;
        }

        private static string GetNewValues(EntityEntry entry)
        {
            if (entry.State == EntityState.Deleted)
            {
                return string.Empty;
            }

            var newValues = new Dictionary<string, object?>();
            foreach (var property in entry.Properties.Where(p => p.CurrentValue != null))
            {
                newValues[property.Metadata.Name] = property.CurrentValue;
            }

            return newValues.Count > 0 ? JsonSerializer.Serialize(newValues) : string.Empty;
        }
    }
}