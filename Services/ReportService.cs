using AutoGestao.Atributes;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models.Report;
using AutoGestao.Services.Interface;
using System.Reflection;
using System.Text;

namespace AutoGestao.Services
{
    public class ReportService : IReportService
    {
        public string GenerateReportHtml<T>(T entity, ReportTemplate template) where T : BaseEntidade, new()
        {
            var config = typeof(T).GetCustomAttribute<ReportConfigAttribute>();
            var html = new StringBuilder();

            // Header
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine($"<title>{config?.Title ?? typeof(T).Name}</title>");
            html.AppendLine("<style>");
            html.AppendLine(GetReportStyles());
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");

            // Header do relatório
            html.AppendLine("<div class='report-header'>");
            html.AppendLine($"<h1>{config?.HeaderText ?? config?.Title ?? "Relatório"}</h1>");

            if (config?.ShowDate ?? true)
            {
                html.AppendLine($"<div class='report-info'>Data: {DateTime.Now:dd/MM/yyyy HH:mm}</div>");
            }

            html.AppendLine("</div>");

            // Processar seções
            var sections = template.Sections.OrderBy(s => s.Order);

            foreach (var section in sections)
            {
                html.AppendLine($"<div class='report-section'>");

                if (!string.IsNullOrEmpty(section.Title))
                {
                    html.AppendLine($"<h2 class='section-title'>{section.Title}</h2>");
                }

                if (!string.IsNullOrEmpty(section.Subtitle))
                {
                    html.AppendLine($"<div class='section-subtitle'>{section.Subtitle}</div>");
                }

                if (section.Type == "grid")
                {
                    html.AppendLine(GenerateGridSection(entity, section));
                }
                else if (section.Type == "table")
                {
                    html.AppendLine(GenerateTableSection(entity, section));
                }

                html.AppendLine("</div>");
            }

            // Botões de ação
            html.AppendLine("<div class='no-print' style='margin-top: 30px; text-align: center;'>");
            html.AppendLine("<button onclick='window.print()' class='btn-print'>Imprimir / Salvar PDF</button>");
            html.AppendLine("<button onclick='window.close()' class='btn-close'>Fechar</button>");
            html.AppendLine("</div>");

            html.AppendLine("</body></html>");

            return html.ToString();
        }

        public ReportTemplate GetDefaultTemplate<T>() where T : BaseEntidade, new()
        {
            var template = new ReportTemplate
            {
                Name = typeof(T).Name,
                Sections = []
            };

            var config = typeof(T).GetCustomAttribute<ReportConfigAttribute>();
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<ReportFieldAttribute>() != null)
                .OrderBy(p => p.GetCustomAttribute<ReportFieldAttribute>()?.Order ?? 0);

            var sections = properties
                .GroupBy(p => p.GetCustomAttribute<ReportFieldAttribute>()?.Section ?? "Dados Gerais")
                .Select((group, index) => new ReportSection
                {
                    Title = group.Key,
                    Type = "grid",
                    Columns = 3,
                    Order = index,
                    Fields = [.. group.Select(p =>
                    {
                        var attr = p.GetCustomAttribute<ReportFieldAttribute>();
                        return new ReportField
                        {
                            Label = attr?.Label ?? p.Name,
                            PropertyName = p.Name,
                            Format = attr?.Format,
                            Order = attr?.Order ?? 0
                        };
                    })]
                });

            template.Sections.AddRange(sections);

            return template;
        }

        public List<ReportFieldInfo> GetReportFields<T>() where T : BaseEntidade, new()
        {
            var fields = new List<ReportFieldInfo>();

            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<ReportFieldAttribute>() != null);

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<ReportFieldAttribute>();
                fields.Add(new ReportFieldInfo
                {
                    PropertyName = prop.Name,
                    Label = attr?.Label ?? prop.Name,
                    Section = attr?.Section ?? "Dados Gerais",
                    Type = attr?.Type ?? EnumReportFieldType.Text,
                    Order = attr?.Order ?? 0
                });
            }

            return fields.OrderBy(f => f.Order).ToList();
        }

        private static string GenerateGridSection<T>(T entity, ReportSection section)
        {
            var html = new StringBuilder();
            var fields = section.Fields.OrderBy(f => f.Order);

            html.AppendLine($"<div class='grid-container' style='grid-template-columns: repeat({section.Columns}, 1fr);'>");

            foreach (var field in fields)
            {
                var value = GetPropertyValue(entity, field.PropertyName);
                var formattedValue = FormatValue(value, field.Format);

                html.AppendLine("<div class='grid-item'>");
                html.AppendLine($"<label>{field.Label}:</label>");
                html.AppendLine($"<span>{formattedValue}</span>");
                html.AppendLine("</div>");
            }

            html.AppendLine("</div>");

            return html.ToString();
        }

        private static string GenerateTableSection<T>(T entity, ReportSection section)
        {
            var html = new StringBuilder();
            var tableData = GetPropertyValue(entity, section.DataProperty) as System.Collections.IEnumerable;

            if (tableData == null)
            {
                return "<div class='alert'>Nenhum dado disponível</div>";
            }

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

            foreach (var row in tableData)
            {
                hasData = true;
                html.AppendLine("<tr>");

                foreach (var column in section.ListColumns)
                {
                    var value = GetPropertyValue(row, column.PropertyName);
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

            if (!hasData)
            {
                html.AppendLine($"<tr><td colspan='{section.ListColumns.Count}' class='text-center'>Nenhum registro encontrado</td></tr>");
            }

            html.AppendLine("</tbody>");

            if (section.ShowTotal && hasData)
            {
                html.AppendLine("<tfoot>");
                html.AppendLine($"<tr><td colspan='{section.ListColumns.Count - 1}' class='text-right'><strong>Total:</strong></td>");
                html.AppendLine($"<td class='text-right'><strong>{total:C2}</strong></td></tr>");
                html.AppendLine("</tfoot>");
            }

            html.AppendLine("</table>");

            return html.ToString();
        }

        private static object? GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            var prop = obj.GetType().GetProperty(propertyName);
            return prop?.GetValue(obj);
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
                    return decValue.ToString(format);
                }

                if (value is int intValue)
                {
                    return intValue.ToString(format);
                }
            }

            if (value is DateTime date)
            {
                return date.ToString("dd/MM/yyyy");
            }

            if (value is decimal dec)
            {
                return dec.ToString("C2");
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
                body { font-family: Arial, sans-serif; padding: 40px; font-size: 12px; }
                .report-header { margin-bottom: 30px; border-bottom: 2px solid #000; padding-bottom: 15px; }
                .report-header h1 { font-size: 24px; margin-bottom: 10px; }
                .report-info { font-size: 11px; color: #666; }
                .report-section { margin-bottom: 30px; page-break-inside: avoid; }
                .section-title { font-size: 18px; font-weight: bold; margin-bottom: 10px; }
                .section-subtitle { font-size: 14px; border-bottom: 1px solid #000; padding-bottom: 5px; margin-bottom: 15px; }
                .grid-container { display: grid; gap: 15px; margin-bottom: 20px; }
                .grid-item { display: flex; gap: 8px; }
                .grid-item label { font-weight: bold; min-width: 120px; }
                .grid-item span { flex: 1; }
                .report-table { width: 100%; border-collapse: collapse; margin-top: 10px; }
                .report-table th, .report-table td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                .report-table th { background-color: #f5f5f5; font-weight: bold; }
                .report-table tfoot td { font-weight: bold; background-color: #f9f9f9; }
                .text-right { text-align: right !important; }
                .text-center { text-align: center !important; }
                .alert { padding: 15px; background-color: #f0f0f0; border: 1px solid #ddd; border-radius: 4px; }
                .btn-print, .btn-close { padding: 12px 24px; margin: 0 5px; border: none; border-radius: 6px; cursor: pointer; font-size: 16px; }
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
    }
}
