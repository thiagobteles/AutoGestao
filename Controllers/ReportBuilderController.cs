using AutoGestao.Data;
using AutoGestao.Entidades.Relatorio;
using AutoGestao.Models.Report;
using AutoGestao.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AutoGestao.Controllers
{
    /// <summary>
    /// Controller para construção visual de templates de relatórios
    /// </summary>
    public class ReportBuilderController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly EntityInspectorService _entityInspector = new(context);

        /// <summary>
        /// Página principal do builder
        /// GET: /ReportBuilder
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Criar novo template
        /// GET: /ReportBuilder/Create
        /// </summary>
        public IActionResult Create(string? entityType)
        {
            ViewBag.EntityType = entityType;
            return View();
        }

        /// <summary>
        /// Editar template existente
        /// GET: /ReportBuilder/Edit/5
        /// </summary>
        public async Task<IActionResult> Edit(long id)
        {
            var template = await _context.ReportTemplates.FindAsync(id);
            if (template == null)
            {
                return NotFound();
            }

            ViewBag.TemplateId = id;
            ViewBag.EntityType = template.TipoEntidade;
            ViewBag.TemplateJson = template.TemplateJson;
            ViewBag.TemplateName = template.Nome;

            return View("Create");
        }

        /// <summary>
        /// Obter lista de entidades disponíveis
        /// GET: /ReportBuilder/GetEntities
        /// </summary>
        [HttpGet]
        public IActionResult GetEntities()
        {
            try
            {
                var entities = _entityInspector.GetAvailableEntities();
                return Json(new { success = true, data = entities });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obter propriedades de uma entidade
        /// GET: /ReportBuilder/GetEntityProperties?entityName=Veiculo
        /// </summary>
        [HttpGet]
        public IActionResult GetEntityProperties(string entityName)
        {
            try
            {
                var entityInfo = _entityInspector.GetEntityInfo(entityName);
                return Json(new { success = true, data = entityInfo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Salvar template
        /// POST: /ReportBuilder/Save
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveTemplateRequest request)
        {
            try
            {
                var templateJson = JsonSerializer.Serialize(request.Template);

                if (request.TemplateId.HasValue && request.TemplateId.Value > 0)
                {
                    // Atualizar template existente
                    var existing = await _context.ReportTemplates.FindAsync(request.TemplateId.Value);
                    if (existing == null)
                    {
                        return Json(new { success = false, message = "Template não encontrado" });
                    }

                    existing.Nome = request.Name;
                    existing.Descricao = request.Description;
                    existing.TemplateJson = templateJson;
                    existing.IsPadrao = request.IsDefault;
                }
                else
                {
                    // Criar novo template
                    var newTemplate = new ReportTemplateEntity
                    {
                        Nome = request.Name,
                        TipoEntidade = request.EntityType,
                        Descricao = request.Description,
                        TemplateJson = templateJson,
                        IsPadrao = request.IsDefault,
                        Ativo = true
                    };

                    _context.ReportTemplates.Add(newTemplate);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Template salvo com sucesso!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao salvar template: {ex.Message}" });
            }
        }

        /// <summary>
        /// Preview do relatório
        /// POST: /ReportBuilder/Preview
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Preview([FromBody] PreviewRequest request)
        {
            try
            {
                // Buscar um registro de exemplo da entidade
                object? sampleEntity = request.EntityType switch
                {
                    "Cliente" => await _context.Clientes.FirstOrDefaultAsync(),
                    "Veiculo" => await _context.Veiculos
                        .Include(v => v.Cliente)
                        .Include(v => v.VeiculoMarca)
                        .Include(v => v.VeiculoMarcaModelo)
                        .Include(v => v.VeiculoCor)
                        .FirstOrDefaultAsync(),
                    _ => null
                };

                if (sampleEntity == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Nenhum registro de {request.EntityType} encontrado para preview"
                    });
                }

                // Gerar HTML usando o template
                var html = Base.ReportController.GenerateReportHtmlDynamic(sampleEntity, request.Template);
                return Json(new { success = true, html });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao gerar preview: {ex.Message}" });
            }
        }
    }
}
