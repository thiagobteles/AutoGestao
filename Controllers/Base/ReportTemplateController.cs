using AutoGestao.Data;
using AutoGestao.Entidades.Relatorio;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers.Base
{
    public class ReportTemplateController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<ReportTemplateEntity>> logger,  IReportService reportService) 
        : StandardGridController<ReportTemplateEntity>(context, fileStorageService, logger, reportService)
    {
        // Sobrescrever para customizar se necessário
        protected override IQueryable<ReportTemplateEntity> GetBaseQuery()
        {
            return base.GetBaseQuery()
                .Where(t => t.Ativo)
                .OrderByDescending(t => t.IsPadrao)
                .ThenBy(t => t.Nome);
        }

        /// <summary>
        /// Buscar templates por tipo de entidade
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetByEntityType(string entityType)
        {
            var templates = await _context.ReportTemplates
                .Where(t => t.TipoEntidade == entityType && t.Ativo)
                .OrderByDescending(t => t.IsPadrao)
                .ThenBy(t => t.Nome)
                .Select(t => new
                    {
                        t.Id,
                        t.Nome,
                        t.Descricao,
                        t.IsPadrao,
                        t.TemplateJson
                    })
                .ToListAsync();

            return Json(templates);
        }

        /// <summary>
        /// Obter template padrão para um tipo de entidade
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDefault(string entityType)
        {
            var template = await _context.ReportTemplates
                .Where(t => t.TipoEntidade == entityType && t.IsPadrao && t.Ativo)
                .FirstOrDefaultAsync();

            if (template == null)
            {
                return NotFound();
            }

            return Json(new
            {
                template.Id,
                template.Nome,
                template.Descricao,
                template.TemplateJson
            });
        }

        /// <summary>
        /// Clonar template
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Clone(long id)
        {
            var original = await _context.ReportTemplates.FindAsync(id);

            if (original == null)
            {
                return NotFound();
            }

            var clone = new ReportTemplateEntity
            {
                Nome = $"{original.Nome} (Cópia)",
                TipoEntidade = original.TipoEntidade,
                Descricao = original.Descricao,
                TemplateJson = original.TemplateJson,
                IsPadrao = false,
                Ativo = true,
                IdEmpresa = original.IdEmpresa
            };

            _context.ReportTemplates.Add(clone);
            await _context.SaveChangesAsync();
            return Json(new { success = true, id = clone.Id });
        }
    }
}