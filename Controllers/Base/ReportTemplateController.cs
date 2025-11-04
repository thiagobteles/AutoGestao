using AutoGestao.Data;
using AutoGestao.Entidades.Relatorio;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers.Base
{
    public class ReportTemplateController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<ReportTemplateEntity>> logger)
        : StandardGridController<ReportTemplateEntity>(context, fileStorageService, logger)
    {
        // Sobrescrever para customizar query base
        protected override IQueryable<ReportTemplateEntity> GetBaseQuery()
        {
            return base.GetBaseQuery()
                .Where(t => t.Ativo)
                .OrderByDescending(t => t.IsPadrao)
                .ThenBy(t => t.Nome);
        }

        /// <summary>
        /// Customizar grid para adicionar botões e ações especiais
        /// </summary>
        protected override StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel gridViewModel)
        {
            var itemNovo = gridViewModel.HeaderActions.FirstOrDefault(x => x.Name == "Create");
            gridViewModel.HeaderActions.Remove(itemNovo);

            // Adicionar botão customizado no header para criar novo template
            gridViewModel.HeaderActions.Add(new GridAction
            {
                Name = "create_template",
                DisplayName = "Criar Novo Template",
                Icon = "fas fa-magic",
                CssClass = "btn btn-primary",
                Url = "/ReportBuilder/Create",
                Type = EnumTypeRequest.Get
            });

            // Desabilitar botões padrão
            gridViewModel.ShowCreateButton = false;
            gridViewModel.ShowEditButton = false;
            gridViewModel.ShowDetailsButton = false;  // Não faz sentido "Details" para templates

            // Limpar RowActions padrão e adicionar apenas os que fazem sentido
            gridViewModel.RowActions.Clear();

            // Adicionar ação customizada para editar no builder
            gridViewModel.RowActions.Add(new GridAction
            {
                Name = "edit_builder",
                DisplayName = "Editar",
                Icon = "fas fa-edit",
                CssClass = "btn btn-sm btn-outline-primary",
                Url = "/ReportBuilder/Edit/{id}",
                Type = EnumTypeRequest.Get
            });

            // Adicionar ação para clonar template
            gridViewModel.RowActions.Add(new GridAction
            {
                Name = "clone_template",
                DisplayName = "Clonar",
                Icon = "fas fa-copy",
                CssClass = "btn btn-sm btn-outline-info",
                Url = "/ReportTemplate/Clone/{id}",
                Type = EnumTypeRequest.Post
            });

            // Manter ação de excluir
            gridViewModel.RowActions.Add(new GridAction
            {
                Name = "Delete",
                DisplayName = "Excluir",
                Icon = "fas fa-trash",
                Url = "/ReportTemplate/Delete/{id}",
                Type = EnumTypeRequest.Post,
                CssClass = "btn btn-sm btn-outline-danger"
            });

            return base.ConfigureCustomGrid(gridViewModel);
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