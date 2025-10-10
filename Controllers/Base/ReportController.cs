using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Models.Report;
using AutoGestao.Services;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AutoGestao.Controllers.Base
{
    public class ReportController(ApplicationDbContext context, IReportService reportService) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IReportService _reportService = reportService;

        /// <summary>
        /// Gerar relatório em HTML/PDF
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Generate(string entityType, long entityId, string templateJson)
        {
            try
            {
                var template = JsonSerializer.Deserialize<ReportTemplate>(templateJson);
                if (template == null)
                {
                    return BadRequest("Template inválido");
                }

                object? entity = entityType switch
                {
                    "Cliente" => await _context.Clientes.FindAsync(entityId),
                    "Veiculo" => await _context.Veiculos
                        .Include(v => v.Cliente)
                        .Include(v => v.VeiculoMarca)
                        .FirstOrDefaultAsync(v => v.Id == entityId),
                    "Venda" => await _context.Vendas
                        .Include(v => v.Cliente)
                        .Include(v => v.Veiculo)
                        .Include(v => v.Parcelas)
                        .FirstOrDefaultAsync(v => v.Id == entityId),
                    _ => null
                };

                if (entity == null)
                {
                    return NotFound();
                }

                var html = GenerateReportHtmlDynamic(entity, template);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar relatório: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera relatório rápido com template padrão
        /// Uso: /Report/Quick?entityType=Cliente&entityId=1
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Quick(string entityType, long entityId)
        {
            try
            {
                // Buscar entidade do banco
                object? entity = await GetEntityByType(entityType, entityId);

                if (entity == null)
                {
                    return NotFound($"{entityType} com ID {entityId} não encontrado");
                }

                // Obter template padrão baseado no tipo
                var template = entityType switch
                {
                    "Clientes" => GetClienteTemplate(),
                    "Veiculos" => GetVeiculoTemplate(),
                    "Vendas" => GetVendaTemplate(),
                    _ => CreateGenericTemplate(entityType)
                };

                // Gerar HTML
                var html = GenerateReportHtmlDynamic(entity, template);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao gerar relatório: {ex.Message}");
            }
        }

        /// <summary>
        /// Buscar templates salvos para uma entidade
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSavedTemplates(string entityType)
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
                    t.IsPadrao
                })
                .ToListAsync();

            return Json(templates);
        }

        /// <summary>
        /// Gerar relatório usando template salvo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerateFromSaved(long templateId, long entityId)
        {
            var templateEntity = await _context.ReportTemplates.FindAsync(templateId);
            if (templateEntity == null)
            {
                return NotFound("Template não encontrado");
            }

            var entity = await GetEntityByType(templateEntity.TipoEntidade, entityId);
            if (entity == null)
            {
                return NotFound($"{templateEntity.TipoEntidade} não encontrado");
            }

            // Deserializar template JSON
            var template = JsonSerializer.Deserialize<ReportTemplate>(templateEntity.TemplateJson);
            if (template == null)
            {
                return BadRequest("Template JSON inválido");
            }

            // Gerar HTML
            var html = GenerateReportHtmlDynamic(entity, template);
            return Content(html, "text/html");
        }

        public static ReportTemplate GetClienteTemplate()
        {
            return new ReportTemplate
            {
                Name = "Relatório de Cliente",
                Sections =
                [
                    new()
                    {
                        Title = "Dados do Cliente",
                        Subtitle = "Informações Cadastrais",
                        Type = "grid",
                        Columns = 3,
                        Order = 0,
                        Fields =
                        [
                            new() { Label = "Nome", PropertyName = "Nome", Order = 0 },
                            new() { Label = "CPF/CNPJ", PropertyName = "Documento", Order = 1 },
                            new() { Label = "Email", PropertyName = "Email", Order = 2 },
                            new() { Label = "Telefone", PropertyName = "Telefone", Order = 3 },
                            new() { Label = "Celular", PropertyName = "Celular", Order = 4 },
                            new() { Label = "Cidade", PropertyName = "Cidade", Order = 5 },
                            new() { Label = "Estado", PropertyName = "Estado", Order = 6 }
                        ]
                    }
                ]
            };
        }

        public static ReportTemplate GetVeiculoTemplate()
        {
            return new ReportTemplate
            {
                Name = "Relatório de Veículo",
                Sections =
                [
                    new()
                    {
                        Title = "Dados do Veículo",
                        Type = "grid",
                        Columns = 3,
                        Order = 0,
                        Fields =
                        [
                            new() { Label = "Marca", PropertyName = "VeiculoMarca.Descricao", Order = 0 },
                            new() { Label = "Modelo", PropertyName = "VeiculoMarcaModelo.Descricao", Order = 1 },
                            new() { Label = "Placa", PropertyName = "Placa", Order = 2 },
                            new() { Label = "Ano Fabricação", PropertyName = "AnoFabricacao", Order = 3 },
                            new() { Label = "Ano Modelo", PropertyName = "AnoModelo", Order = 4 },
                            new() { Label = "Chassi", PropertyName = "Chassi", Order = 5 },
                            new() { Label = "Cor", PropertyName = "VeiculoCor.Descricao", Order = 6 },
                            new() { Label = "Km", PropertyName = "KmEntrada", Order = 7 }
                        ]
                    }
                ]
            };
        }

        public static ReportTemplate GetVendaTemplate()
        {
            return new ReportTemplate
            {
                Name = "Relatório de Venda",
                Sections =
                [
                    new()
                    {
                        Title = "Dados da Venda",
                        Type = "grid",
                        Columns = 2,
                        Order = 0,
                        Fields =
                        [
                            new() { Label = "Data", PropertyName = "DataVenda", Format = "dd/MM/yyyy", Order = 0 },
                            new() { Label = "Valor Total", PropertyName = "ValorTotal", Format = "C2", Order = 1 },
                            new() { Label = "Cliente", PropertyName = "Cliente.Nome", Order = 2 },
                            new() { Label = "Veículo", PropertyName = "Veiculo.Modelo", Order = 3 }
                        ]
                    },
                    new()
                    {
                        Title = "Parcelas",
                        Type = "table",
                        Order = 1,
                        DataProperty = "Parcelas",
                        ShowTotal = true,
                        TotalField = "Valor",
                        ListColumns =
                        [
                            new() { Label = "Parcela", PropertyName = "NumeroParcela" },
                            new() { Label = "Vencimento", PropertyName = "DataVencimento", Format = "dd/MM/yyyy" },
                            new() { Label = "Status", PropertyName = "Status" },
                            new() { Label = "Valor", PropertyName = "Valor", Format = "C2", Align = "right" }
                        ]
                    }
                ]
            };
        }

        #region Métodos Privados

        /// <summary>
        /// Busca entidade por tipo e ID com includes necessários
        /// </summary>
        private async Task<object?> GetEntityByType(string entityType, long entityId)
        {
            return entityType switch
            {
                "Cliente" => await _context.Clientes
                    .Include(c => c.Vendas)
                    .FirstOrDefaultAsync(c => c.Id == entityId),

                "Veiculo" => await _context.Veiculos
                    .Include(v => v.Cliente)
                    .Include(v => v.VeiculoMarca)
                    .Include(v => v.VeiculoMarcaModelo)
                    .Include(v => v.VeiculoCor)
                    .FirstOrDefaultAsync(v => v.Id == entityId),

                "Venda" => await _context.Vendas
                    .Include(v => v.Cliente)
                    .Include(v => v.Veiculo)
                    .Include(v => v.Parcelas)
                    .FirstOrDefaultAsync(v => v.Id == entityId),

                "Fornecedor" => await _context.Fornecedores
                    .FirstOrDefaultAsync(f => f.Id == entityId),

                "Usuario" => await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == entityId),

                _ => null
            };
        }

        /// <summary>
        /// Cria template genérico para entidades não configuradas
        /// </summary>
        private static ReportTemplate CreateGenericTemplate(string entityType)
        {
            return new ReportTemplate
            {
                Name = $"Relatório de {entityType}",
                Sections =
                [
                    new()
                    {
                        Title = "Dados",
                        Type = "grid",
                        Columns = 2,
                        Order = 0,
                        Fields =
                        [
                            new() { Label = "Id", PropertyName = "Id", Order = 0 },
                            new() { Label = "Nome", PropertyName = "Nome", Order = 1 },
                            new() { Label = "Data Cadastro", PropertyName = "DataCadastro", Format = "dd/MM/yyyy", Order = 2 },
                            new() { Label = "Status", PropertyName = "Ativo", Order = 3 }
                        ]
                    }
                ]
            };
        }

        private static string GenerateReportHtmlDynamic(object entity, ReportTemplate template)
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

                        html.AppendLine("<div class='grid-item'>");
                        html.AppendLine($"<label>{field.Label}:</label>");
                        html.AppendLine($"<span>{formattedValue}</span>");
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

                html.AppendLine("</div>");
            }

            // Botões
            html.AppendLine("<div class='no-print action-buttons'>");
            html.AppendLine("<button onclick='window.print()' class='btn-print'>Imprimir / Salvar PDF</button>");
            html.AppendLine("<button onclick='window.close()' class='btn-close'>Fechar</button>");
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
                * { margin: 0; padding: 0; box-sizing: border-box; }
                body { font-family: Arial, sans-serif; padding: 40px; font-size: 12px; line-height: 1.6; }
                .report-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; border-bottom: 2px solid #000; padding-bottom: 15px; }
                .report-header h1 { font-size: 24px; font-weight: bold; }
                .report-header h2 { font-size: 20px; text-align: right; }
                .report-info { font-size: 11px; color: #666; margin-bottom: 30px; }
                .report-section { margin-bottom: 30px; page-break-inside: avoid; }
                .section-title { font-size: 16px; font-weight: bold; margin-bottom: 10px; color: #333; }
                .section-subtitle { font-size: 13px; font-weight: bold; border-bottom: 1px solid #000; padding-bottom: 5px; margin-bottom: 15px; }
                .grid-container { display: grid; gap: 12px; margin-bottom: 20px; }
                .grid-item { display: flex; gap: 8px; line-height: 1.6; }
                .grid-item label { font-weight: bold; min-width: 140px; }
                .grid-item span { flex: 1; }
                .report-table { width: 100%; border-collapse: collapse; margin-top: 10px; }
                .report-table th, .report-table td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                .report-table th { background-color: #f5f5f5; font-weight: bold; font-size: 11px; }
                .report-table td { font-size: 11px; }
                .report-table tfoot td { font-weight: bold; background-color: #f9f9f9; border-top: 2px solid #000; }
                .text-right { text-align: right !important; }
                .text-center { text-align: center !important; }
                .action-buttons { margin-top: 30px; text-align: center; }
                .btn-print, .btn-close { padding: 12px 24px; margin: 0 5px; border: none; border-radius: 6px; cursor: pointer; font-size: 16px; font-weight: 500; }
                .btn-print { background: #000; color: #fff; }
                .btn-close { background: #666; color: #fff; }
                .btn-print:hover { background: #333; }
                .btn-close:hover { background: #888; }
                @media print {
                    body { padding: 20px; }
                    .no-print { display: none !important; }
                    .report-section { page-break-inside: avoid; }
                }
            ";
        }

        #endregion Métodos Privados
    }
}