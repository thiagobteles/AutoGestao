using AutoGestao.Controllers;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Extensions
{
    public static class StandardGridControllerExtensions
    {
        /// <summary>
        /// Adiciona histórico de auditoria ao ViewBag para ser exibido na view de Details
        /// </summary>
        public static async Task AddAuditHistoryToViewBag<T>(
            this StandardGridController<T> controller,
            ApplicationDbContext context,
            T entity) where T : BaseEntidade, new()
        {
            try
            {
                var entityName = typeof(T).Name;
                var entityId = GetEntityId(entity);

                if (!string.IsNullOrEmpty(entityId))
                {
                    var auditHistory = await AuditHelper.GetEntityAuditHistory(context, entityName, entityId, 20);
                    controller.ViewBag.AuditLogs = auditHistory;
                    controller.ViewBag.AuditCount = await AuditHelper.GetEntityAuditCount(context, entityName, entityId);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Erro ao carregar histórico de auditoria: {ex.Message}");
                controller.ViewBag.AuditLogs = new List<AutoGestao.Entidades.AuditLog>();
                controller.ViewBag.AuditCount = 0;
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
    }
}