using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Interfaces;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System.Reflection;

namespace AutoGestao.Data
{
    public class AuditInterceptor(IAuditService auditService) : SaveChangesInterceptor
    {
        private readonly IAuditService _auditService = auditService;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                await ProcessAuditAsync(eventData.Context);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task ProcessAuditAsync(DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity.GetType().GetCustomAttribute<AuditableAttribute>() != null)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                await ProcessEntityAsync(entry);
            }
        }

        private async Task ProcessEntityAsync(EntityEntry entry)
        {
            var entityType = entry.Entity.GetType();
            var auditableAttr = entityType.GetCustomAttribute<AuditableAttribute>();

            if (auditableAttr == null)
            {
                return;
            }

            var entidadeNome = entityType.Name;
            var entidadeId = GetEntityId(entry.Entity);

            if (string.IsNullOrEmpty(entidadeId))
            {
                return;
            }

            try
            {
                switch (entry.State)
                {
                    case EntityState.Added when auditableAttr.AuditCreate:
                        await _auditService.LogAsync(
                            entidadeNome,
                            entidadeId,
                            EnumTipoOperacaoAuditoria.Create,
                            valoresNovos: GetEntityValues(entry.Entity)
                        );
                        break;

                    case EntityState.Modified when auditableAttr.AuditUpdate:
                        var camposAlterados = GetChangedFields(entry);
                        if (camposAlterados.Any())
                        {
                            await _auditService.LogAsync(
                                entidadeNome,
                                entidadeId,
                                EnumTipoOperacaoAuditoria.Update,
                                valoresAntigos: GetOriginalValues(entry),
                                valoresNovos: GetCurrentValues(entry),
                                camposAlterados: camposAlterados.ToArray()
                            );
                        }
                        break;

                    case EntityState.Deleted when auditableAttr.AuditDelete:
                        await _auditService.LogAsync(
                            entidadeNome,
                            entidadeId,
                            EnumTipoOperacaoAuditoria.Delete,
                            valoresAntigos: GetEntityValues(entry.Entity)
                        );
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the save operation
                Console.WriteLine($"Erro na auditoria: {ex.Message}");
            }
        }

        private static string GetEntityId(object entity)
        {
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                var value = idProperty.GetValue(entity);
                return value?.ToString() ?? "";
            }
            return "";
        }

        private static Dictionary<string, object?> GetEntityValues(object entity)
        {
            var entityType = entity.GetType();
            var properties = entityType.GetProperties()
                .Where(p => p.CanRead)
                .Where(p => p.GetCustomAttribute<AuditIgnoreAttribute>() == null)
                .ToDictionary(
                    p => p.Name,
                    p => GetPropertyValue(p, entity)
                );

            return properties;
        }

        private static object? GetPropertyValue(PropertyInfo property, object entity)
        {
            var value = property.GetValue(entity);

            // Verificar se é campo sensível
            var sensitiveAttr = property.GetCustomAttribute<AuditSensitiveAttribute>();
            if (sensitiveAttr != null && value != null)
            {
                return sensitiveAttr.MaskPattern;
            }

            // Tratar valores especiais
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (value is decimal || value is double || value is float)
            {
                return value.ToString();
            }

            return value;
        }

        private static List<string> GetChangedFields(EntityEntry entry)
        {
            return entry.Properties
                .Where(p => p.IsModified)
                .Where(p => p.Metadata.PropertyInfo?.GetCustomAttribute<AuditIgnoreAttribute>() == null)
                .Select(p => p.Metadata.Name)
                .ToList();
        }

        private static Dictionary<string, object?> GetOriginalValues(EntityEntry entry)
        {
            var values = new Dictionary<string, object?>();

            foreach (var property in entry.Properties.Where(p => p.IsModified))
            {
                if (property.Metadata.PropertyInfo?.GetCustomAttribute<AuditIgnoreAttribute>() != null)
                {
                    continue;
                }

                var originalValue = property.OriginalValue;
                var propertyInfo = property.Metadata.PropertyInfo;

                if (propertyInfo?.GetCustomAttribute<AuditSensitiveAttribute>() != null && originalValue != null)
                {
                    values[property.Metadata.Name] = "***";
                }
                else
                {
                    values[property.Metadata.Name] = originalValue;
                }
            }

            return values;
        }

        private static Dictionary<string, object?> GetCurrentValues(EntityEntry entry)
        {
            var values = new Dictionary<string, object?>();

            foreach (var property in entry.Properties.Where(p => p.IsModified))
            {
                if (property.Metadata.PropertyInfo?.GetCustomAttribute<AuditIgnoreAttribute>() != null)
                {
                    continue;
                }

                var currentValue = property.CurrentValue;
                var propertyInfo = property.Metadata.PropertyInfo;

                if (propertyInfo?.GetCustomAttribute<AuditSensitiveAttribute>() != null && currentValue != null)
                {
                    values[property.Metadata.Name] = "***";
                }
                else
                {
                    values[property.Metadata.Name] = currentValue;
                }
            }

            return values;
        }
    }
}