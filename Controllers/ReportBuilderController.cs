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
                var html = GenerateReportHtmlDynamic(sampleEntity, request.Template);
                return Json(new { success = true, html });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao gerar preview: {ex.Message}" });
            }
        }

        public string GenerateReportHtmlDynamic(object entity, ReportTemplate template)
        {
            var html = new System.Text.StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine($"<title>{template.Name}</title>");
            html.AppendLine("<style>");
            html.AppendLine(GetReportStyles());
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");

            // Header
            html.AppendLine("<div class='report-header'>");
            html.AppendLine("<div class='header-left'>");
            html.AppendLine("<h1>GRIFFE MOTORS SELECT</h1>");
            html.AppendLine("</div>");
            html.AppendLine("<div class='header-right'>");
            html.AppendLine($"<h2>{template.Name}</h2>");
            html.AppendLine("</div>");
            html.AppendLine("</div>");

            html.AppendLine($"<div class='report-info'>{DateTime.Now:dd/MM/yyyy HH:mm}</div>");

            // Seções
            foreach (var section in template.Sections.OrderBy(s => s.Order))
            {
                html.AppendLine("<div class='report-section'>");

                if (!string.IsNullOrEmpty(section.Title))
                {
                    html.AppendLine($"<h3 class='section-title'>{section.Title}</h3>");
                }

                if (!string.IsNullOrEmpty(section.Subtitle))
                {
                    html.AppendLine($"<div class='section-subtitle'>{section.Subtitle}</div>");
                }

                if (section.Type == "grid")
                {
                    html.AppendLine($"<div class='grid-container' style='grid-template-columns: repeat({section.Columns}, 1fr);'>");

                    foreach (var field in section.Fields.OrderBy(f => f.Order))
                    {
                        var value = GetNestedPropertyValue(entity, field.PropertyName);
                        var formattedValue = FormatValue(value, field.Format);
                        var columnSpan = field.ColumnSpan ?? 1;
                        var boldClass = field.Bold ? "font-weight-bold" : "";
                        var displayClass = field.DisplayType == "badge" ? "field-badge" : field.DisplayType == "highlight" ? "field-highlight" : "";

                        html.AppendLine($"<div class='grid-item {boldClass}' style='grid-column: span {columnSpan};'>");
                        html.AppendLine($"<label>{field.Label}:</label>");
                        html.AppendLine($"<span class='{displayClass}'>{formattedValue}</span>");
                        html.AppendLine("</div>");
                    }

                    html.AppendLine("</div>");
                }
                else if (section.Type == "row")
                {
                    html.AppendLine("<div class='row-container'>");

                    foreach (var field in section.Fields.OrderBy(f => f.Order))
                    {
                        var value = GetNestedPropertyValue(entity, field.PropertyName);
                        var formattedValue = FormatValue(value, field.Format);
                        var boldClass = field.Bold ? "font-weight-bold" : "";
                        var displayClass = field.DisplayType == "badge" ? "field-badge" : field.DisplayType == "highlight" ? "field-highlight" : "";

                        html.AppendLine($"<div class='row-item {boldClass}'>");
                        html.AppendLine($"<label>{field.Label}:</label>");
                        html.AppendLine($"<span class='{displayClass}'>{formattedValue}</span>");
                        html.AppendLine("</div>");
                    }

                    html.AppendLine("</div>");
                }
                else if (section.Type == "table")
                {
                    var tableData = GetNestedPropertyValue(entity, section.DataProperty) as System.Collections.IEnumerable;

                    html.AppendLine("<table class='report-table'>");
                    html.AppendLine("<thead><tr>");

                    foreach (var column in section.ListColumns)
                    {
                        var alignClass = column.Align == "right" ? "text-right" : "";
                        html.AppendLine($"<th class='{alignClass}'>{column.Label}</th>");
                    }

                    html.AppendLine("</tr></thead><tbody>");

                    decimal total = 0;
                    var hasData = false;

                    if (tableData != null)
                    {
                        foreach (var row in tableData)
                        {
                            hasData = true;
                            html.AppendLine("<tr>");

                            foreach (var column in section.ListColumns)
                            {
                                var value = GetNestedPropertyValue(row, column.PropertyName);
                                var formattedValue = FormatValue(value, column.Format);
                                var alignClass = column.Align == "right" ? "text-right" : "";

                                if (section.ShowTotal && column.PropertyName == section.TotalField && value is decimal decValue)
                                {
                                    total += decValue;
                                }

                                html.AppendLine($"<td class='{alignClass}'>{formattedValue}</td>");
                            }

                            html.AppendLine("</tr>");
                        }
                    }

                    if (!hasData)
                    {
                        html.AppendLine($"<tr><td colspan='{section.ListColumns.Count}' class='text-center'>Nenhum registro encontrado</td></tr>");
                    }

                    html.AppendLine("</tbody>");

                    if (section.ShowTotal && hasData)
                    {
                        html.AppendLine("<tfoot>");
                        html.AppendLine($"<tr><td colspan='{section.ListColumns.Count - 1}' class='text-right'><strong>Total:</strong></td>");
                        html.AppendLine($"<td class='text-right'><strong>R$ {total:N2}</strong></td></tr>");
                        html.AppendLine("</tfoot>");
                    }

                    html.AppendLine("</table>");
                }
                else if (section.Type == "richtext")
                {
                    // Renderizar conteúdo HTML do editor de texto rico
                    if (!string.IsNullOrEmpty(section.RichTextContent))
                    {
                        html.AppendLine("<div class='richtext-content'>");
                        html.AppendLine(section.RichTextContent);
                        html.AppendLine("</div>");
                    }
                }
                else if (section.Type == "external_query")
                {
                    // Executar consulta SQL externa
                    if (!string.IsNullOrEmpty(section.SqlQuery))
                    {
                        try
                        {
                            var queryResults = ExecuteExternalQuery(section.SqlQuery, entity);
                            html.AppendLine(RenderQueryResults(queryResults));
                        }
                        catch (Exception ex)
                        {
                            html.AppendLine($"<div class='alert alert-danger'>Erro ao executar consulta: {ex.Message}</div>");
                        }
                    }
                }

                html.AppendLine("</div>");
            }

            // Botões
            html.AppendLine("<div class='no-print action-buttons'>");
            html.AppendLine("<button onclick='window.print()' class='btn-print'>Imprimir / Salvar PDF</button>");
            html.AppendLine("</div>");

            html.AppendLine("</body></html>");

            return html.ToString();
        }

        private static object? GetNestedPropertyValue(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            var parts = propertyPath.Split('.');
            var current = obj;

            foreach (var part in parts)
            {
                var prop = current?.GetType().GetProperty(part);
                if (prop == null)
                {
                    return null;
                }

                current = prop.GetValue(current);
                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }

        private static string FormatValue(object? value, string? format)
        {
            if (value == null)
            {
                return "-";
            }

            if (!string.IsNullOrEmpty(format))
            {
                if (value is DateTime dateValue)
                {
                    return dateValue.ToString(format);
                }

                if (value is decimal decValue)
                {
                    return format == "C2" ? $"R$ {decValue:N2}" : decValue.ToString(format);
                }
            }

            if (value is DateTime date)
            {
                return date.ToString("dd/MM/yyyy");
            }

            if (value is decimal dec)
            {
                return $"R$ {dec:N2}";
            }

            if (value is bool boolean)
            {
                return boolean ? "Sim" : "Não";
            }

            return value.ToString() ?? "-";
        }

        private static string GetReportStyles()
        {
            return @"
                /* ===== RESET E BASE ===== */
                * {
                    margin: 0;
                    padding: 0;
                    box-sizing: border-box;
                }

                body {
                    font-family: 'Segoe UI', 'Helvetica Neue', Arial, sans-serif;
                    padding: 40px;
                    font-size: 13px;
                    line-height: 1.6;
                    color: #2c3e50;
                    background: #f8f9fa;
                }

                /* ===== CABEÇALHO DO RELATÓRIO ===== */
                .report-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 30px;
                    padding: 25px 30px;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    border-radius: 12px;
                    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
                }

                .header-left h1 {
                    font-size: 28px;
                    font-weight: 700;
                    letter-spacing: -0.5px;
                    margin-bottom: 5px;
                }

                .header-right h2 {
                    font-size: 22px;
                    font-weight: 600;
                    text-align: right;
                    opacity: 0.95;
                }

                .report-info {
                    font-size: 12px;
                    color: #7f8c8d;
                    margin-bottom: 30px;
                    padding: 12px 20px;
                    background: white;
                    border-left: 4px solid #667eea;
                    border-radius: 4px;
                    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
                }

                /* ===== SEÇÕES DO RELATÓRIO ===== */
                .report-section {
                    margin-bottom: 35px;
                    page-break-inside: avoid;
                    background: white;
                    padding: 25px;
                    border-radius: 10px;
                    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
                    transition: box-shadow 0.3s ease;
                }

                .report-section:hover {
                    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.12);
                }

                .section-title {
                    font-size: 20px;
                    font-weight: 700;
                    margin-bottom: 15px;
                    color: #2c3e50;
                    padding-bottom: 12px;
                    border-bottom: 3px solid #667eea;
                    display: flex;
                    align-items: center;
                    gap: 10px;
                }

                .section-title i {
                    color: #667eea;
                }

                .section-subtitle {
                    font-size: 14px;
                    font-weight: 600;
                    color: #7f8c8d;
                    margin-bottom: 20px;
                    padding-bottom: 8px;
                    border-bottom: 1px solid #ecf0f1;
                }

                /* ===== GRID LAYOUT ===== */
                .grid-container {
                    display: grid;
                    gap: 18px;
                    margin-bottom: 20px;
                }

                .grid-item {
                    display: flex;
                    flex-direction: column;
                    gap: 6px;
                    padding: 12px 15px;
                    background: #f8f9fa;
                    border-radius: 8px;
                    border-left: 3px solid #e9ecef;
                    transition: all 0.2s ease;
                }

                .grid-item:hover {
                    background: #f1f3f5;
                    border-left-color: #667eea;
                }

                .grid-item label {
                    font-weight: 600;
                    font-size: 11px;
                    text-transform: uppercase;
                    letter-spacing: 0.5px;
                    color: #7f8c8d;
                }

                .grid-item span {
                    font-size: 14px;
                    font-weight: 500;
                    color: #2c3e50;
                }

                .grid-item.font-weight-bold span {
                    font-weight: 700;
                    color: #1a252f;
                }

                /* ===== ROW LAYOUT ===== */
                .row-container {
                    margin-bottom: 20px;
                }

                .row-item {
                    display: grid;
                    grid-template-columns: 200px 1fr;
                    gap: 15px;
                    padding: 15px 20px;
                    background: #f8f9fa;
                    border-radius: 8px;
                    margin-bottom: 12px;
                    border-left: 4px solid #667eea;
                }

                .row-item label {
                    font-weight: 700;
                    font-size: 13px;
                    color: #2c3e50;
                }

                .row-item span {
                    font-size: 14px;
                    color: #495057;
                }

                /* ===== FIELD STYLES ===== */
                .field-badge {
                    display: inline-block;
                    padding: 4px 12px;
                    background: #667eea;
                    color: white;
                    border-radius: 20px;
                    font-size: 12px;
                    font-weight: 600;
                }

                .field-highlight {
                    background: #fff3cd;
                    padding: 4px 8px;
                    border-radius: 4px;
                    border-left: 3px solid #ffc107;
                    font-weight: 600;
                }

                /* ===== TABELAS ===== */
                .report-table {
                    width: 100%;
                    border-collapse: separate;
                    border-spacing: 0;
                    margin-top: 15px;
                    background: white;
                    border-radius: 8px;
                    overflow: hidden;
                    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.06);
                }

                .report-table thead {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                }

                .report-table th {
                    padding: 14px 12px;
                    text-align: left;
                    font-weight: 600;
                    font-size: 12px;
                    text-transform: uppercase;
                    letter-spacing: 0.5px;
                    border: none;
                }

                .report-table td {
                    padding: 12px;
                    border-bottom: 1px solid #ecf0f1;
                    font-size: 13px;
                }

                .report-table tbody tr {
                    transition: background-color 0.2s ease;
                }

                .report-table tbody tr:hover {
                    background-color: #f8f9fa;
                }

                .report-table tbody tr:last-child td {
                    border-bottom: none;
                }

                .report-table tfoot td {
                    font-weight: 700;
                    background: #f8f9fa;
                    border-top: 3px solid #667eea;
                    padding: 14px 12px;
                    font-size: 14px;
                }

                /* ===== UTILITÁRIOS ===== */
                .text-right { text-align: right !important; }
                .text-center { text-align: center !important; }

                /* ===== BOTÕES DE AÇÃO ===== */
                .action-buttons {
                    margin-top: 40px;
                    text-align: center;
                    padding: 20px;
                }

                .btn-print, .btn-close {
                    padding: 14px 32px;
                    margin: 0 8px;
                    border: none;
                    border-radius: 8px;
                    cursor: pointer;
                    font-size: 15px;
                    font-weight: 600;
                    transition: all 0.3s ease;
                    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                }

                .btn-print {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                }

                .btn-print:hover {
                    transform: translateY(-2px);
                    box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
                }

                /* ===== CONTEÚDO DE TEXTO RICO ===== */
                .richtext-content {
                    padding: 15px;
                    background: #f8f9fa;
                    border-radius: 8px;
                    line-height: 1.8;
                    color: #2c3e50;
                }

                .richtext-content img {
                    max-width: 100%;
                    height: auto;
                    border-radius: 4px;
                    margin: 10px 0;
                }

                .richtext-content ul,
                .richtext-content ol {
                    margin-left: 25px;
                    margin-bottom: 15px;
                }

                .richtext-content p {
                    margin-bottom: 10px;
                }

                /* ===== MENSAGENS DE ALERTA ===== */
                .alert {
                    padding: 15px 20px;
                    border-radius: 8px;
                    margin: 15px 0;
                    font-size: 14px;
                    font-weight: 500;
                }

                .alert-danger {
                    background: #fee;
                    color: #c33;
                    border: 2px solid #fcc;
                }

                .alert-info {
                    background: #e7f3ff;
                    color: #0066cc;
                    border: 2px solid #99ccff;
                }

                /* ===== IMPRESSÃO ===== */
                @media print {
                    body {
                        padding: 15px;
                        background: white;
                    }

                    .no-print {
                        display: none !important;
                    }

                    .report-section {
                        page-break-inside: avoid;
                        box-shadow: none;
                        border: 1px solid #dee2e6;
                    }

                    .report-header {
                        background: #667eea !important;
                        -webkit-print-color-adjust: exact;
                        print-color-adjust: exact;
                    }

                    .report-table thead {
                        background: #667eea !important;
                        -webkit-print-color-adjust: exact;
                        print-color-adjust: exact;
                    }
                }

                /* ===== ANIMAÇÕES ===== */
                @keyframes fadeIn {
                    from {
                        opacity: 0;
                        transform: translateY(10px);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                    }
                }

                .report-section {
                    animation: fadeIn 0.4s ease-out;
                }
            ";
        }

        /// <summary>
        /// Executar consulta SQL externa com substituição do parâmetro @Id
        /// </summary>
        private List<Dictionary<string, object>> ExecuteExternalQuery(string sqlQuery, object entity)
        {
            // Obter o ID da entidade
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("A entidade não possui propriedade 'Id'");
            }

            var entityId = idProperty.GetValue(entity);
            if (entityId == null)
            {
                throw new InvalidOperationException("O ID da entidade não pode ser nulo");
            }

            // Substituir @Id pelo valor real
            var query = sqlQuery.Replace("@Id", entityId.ToString());

            // Executar a consulta
            var results = new List<Dictionary<string, object>>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                _context.Database.OpenConnection();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var columnName = reader.GetName(i);
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row[columnName] = value;
                    }
                    results.Add(row);
                }
            }

            return results;
        }

        /// <summary>
        /// Renderizar resultados da consulta SQL em HTML
        /// </summary>
        private static string RenderQueryResults(List<Dictionary<string, object>> results)
        {
            var html = new System.Text.StringBuilder();

            if (results == null || results.Count == 0)
            {
                html.AppendLine("<div class='alert alert-info'>Nenhum resultado encontrado</div>");
                return html.ToString();
            }

            // Obter nomes das colunas do primeiro resultado
            var columns = results[0].Keys.ToList();

            // Iniciar tabela
            html.AppendLine("<table class='report-table'>");
            html.AppendLine("<thead><tr>");

            // Cabeçalhos das colunas
            foreach (var column in columns)
            {
                html.AppendLine($"<th>{column}</th>");
            }

            html.AppendLine("</tr></thead><tbody>");

            // Linhas de dados
            foreach (var row in results)
            {
                html.AppendLine("<tr>");
                foreach (var column in columns)
                {
                    var value = row[column];
                    var formattedValue = FormatValue(value, null);
                    html.AppendLine($"<td>{formattedValue}</td>");
                }
                html.AppendLine("</tr>");
            }

            html.AppendLine("</tbody></table>");

            return html.ToString();
        }
    }
}