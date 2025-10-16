using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Helpers;
using AutoGestao.Models;
using AutoGestao.Models.Auth;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers.Base
{
    [Authorize(Roles = "Admin,Gerente")]
    public class AuditLogController(ApplicationDbContext context, IAuditService auditService, IFileStorageService fileStorageService, IReportService reportService) 
        : StandardGridController<AuditLog>(context, fileStorageService, reportService)
    {
        private readonly IAuditService _auditService = auditService;

        protected override IQueryable<AuditLog> GetBaseQuery()
        {
            return _context.AuditLogs
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.DataHora);
        }

        protected override List<SelectListItem> GetCustomSelectOptions(string propertyName)
        {
            return propertyName switch
            {
                nameof(AuditLog.UsuarioId) => [.. _context.Usuarios
                    .Where(u => u.Ativo)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Nome
                    })],

                "EntidadeNome" => [.. _context.AuditLogs
                    .Select(a => a.EntidadeNome)
                    .Distinct()
                    .OrderBy(e => e)
                    .Select(e => new SelectListItem { Value = e, Text = e })],

                _ => base.GetCustomSelectOptions(propertyName)
            };
        }

        // Sobrescrever para desabilitar criação, edição e exclusão
        public override Task<IActionResult> Create()
        {
            TempData["NotificationScript"] = "showError('Logs de auditoria não podem ser criados manualmente.')";
            return Task.FromResult<IActionResult>(RedirectToAction(nameof(Index)));
        }

        public override Task<IActionResult> Edit(long? id)
        {
            TempData["NotificationScript"] = "showError('Logs de auditoria não podem ser editados.')";
            return Task.FromResult<IActionResult>(RedirectToAction(nameof(Index)));
        }

        public override Task<IActionResult> Delete(long id)
        {
            TempData["NotificationScript"] = "showError('Logs de auditoria não podem ser excluídos.')";
            return Task.FromResult<IActionResult>(RedirectToAction(nameof(Index)));
        }

        protected override bool CanCreate(AuditLog entity)
        {
            return false; // Auditoria é read-only
        }

        protected override bool CanEdit(AuditLog entity)
        {
            return false; // Auditoria é read-only
        }

        protected override bool CanDelete(AuditLog entity)
        {
            return false; // Auditoria é read-only
        }

        protected override void ConfigureFormFields(List<FormFieldViewModel> fields, AuditLog entity, string action)
        {
            // Formatar campos especiais para visualização
            if (action == "Details")
            {
                // Formatar JSON de valores
                var valoresAntigosField = fields.FirstOrDefault(f => f.PropertyName == nameof(AuditLog.ValoresAntigos));
                if (valoresAntigosField != null && !string.IsNullOrEmpty(entity.ValoresAntigos))
                {
                    valoresAntigosField.Value = FormatJson(entity.ValoresAntigos);
                }

                var valoresNovosField = fields.FirstOrDefault(f => f.PropertyName == nameof(AuditLog.ValoresNovos));
                if (valoresNovosField != null && !string.IsNullOrEmpty(entity.ValoresNovos))
                {
                    valoresNovosField.Value = FormatJson(entity.ValoresNovos);
                }

                // Adicionar link para a entidade auditada (se existir)
                if (!string.IsNullOrEmpty(entity.EntidadeNome) && !string.IsNullOrEmpty(entity.EntidadeId))
                {
                    var linkField = new FormFieldViewModel
                    {
                        PropertyName = "LinkEntidade",
                        DisplayName = "Ver Entidade",
                        Value = GetEntityLink(entity.EntidadeNome, entity.EntidadeId),
                        ReadOnly = true,
                        Section = "Navegação",
                        Icon = "fas fa-external-link-alt"
                    };
                    fields.Add(linkField);
                }

                // Adicionar informações de duração se disponível
                if (entity.DuracaoMs.HasValue)
                {
                    var duracaoField = fields.FirstOrDefault(f => f.PropertyName == nameof(AuditLog.DuracaoMs));
                    if (duracaoField != null)
                    {
                        duracaoField.Value = $"{entity.DuracaoMs}ms";
                        if (entity.DuracaoMs > 1000)
                        {
                            duracaoField.Value += $" ({entity.DuracaoMs / 1000.0:F2}s)";
                        }
                    }
                }
            }
        }

        #region Ações personalizadas

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var ultimosDias = 30;
                var dataInicio = DateTime.UtcNow.AddDays(-ultimosDias);

                var estatisticas = await _context.AuditLogs
                    .Where(a => a.DataHora >= dataInicio)
                    .GroupBy(a => a.TipoOperacao)
                    .Select(g => new { Operacao = g.Key, Total = g.Count() })
                    .ToListAsync();

                var operacoesPorDia = await _context.AuditLogs
                    .Where(a => a.DataHora >= dataInicio)
                    .GroupBy(a => a.DataHora.Date)
                    .Select(g => new { Data = g.Key, Total = g.Count() })
                    .OrderBy(x => x.Data)
                    .ToListAsync();

                // Converter para ValueTuple após trazer do banco
                var estatisticasTuple = estatisticas.Select(e => (e.Operacao, e.Total)).ToList();
                var operacoesPorDiaTuple = operacoesPorDia.Select(o => (o.Data, o.Total)).ToList();

                var viewModel = new DashboardAuditViewModel
                {
                    EstatisticasPorOperacao = estatisticasTuple,
                    OperacoesPorDia = operacoesPorDiaTuple,
                    TotalOperacoes = estatisticas.Sum(e => e.Total),
                    UltimosDias = ultimosDias
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["NotificationScript"] = $"showError('Erro ao carregar dashboard: {ex.Message}')";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> RelatorioUsuario(int usuarioId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            dataInicio ??= DateTime.UtcNow.AddDays(-30);
            dataFim ??= DateTime.UtcNow;

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            var logs = await _auditService.GetLogsAsync(
                usuarioId: usuarioId,
                dataInicio: dataInicio,
                dataFim: dataFim,
                take: 1000
            );

            var viewModel = new
            {
                Usuario = usuario,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Logs = logs,
                TotalOperacoes = logs.Count,
                OperacoesPorTipo = logs.GroupBy(l => l.TipoOperacao).ToDictionary(g => g.Key, g => g.Count()),
                EntidadesAfetadas = logs.GroupBy(l => l.EntidadeNome).ToDictionary(g => g.Key, g => g.Count())
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ExportarCSV(
            int? usuarioId = null,
            string? entidade = null,
            EnumTipoOperacaoAuditoria? tipoOperacao = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null)
        {
            try
            {
                var logs = await _auditService.GetLogsAsync(
                    usuarioId, entidade, tipoOperacao, dataInicio, dataFim, take: 10000
                );

                var csv = GenerateCSV(logs);
                var fileName = $"auditoria_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                // Log da exportação
                await _auditService.LogAsync(
                    "AuditLog",
                    "Export",
                    EnumTipoOperacaoAuditoria.Export,
                    valoresNovos: new
                    {
                        TotalRegistros = logs.Count,
                        Filtros = new { usuarioId, entidade, tipoOperacao, dataInicio, dataFim }
                    }
                );

                return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["NotificationScript"] = $"showError('Erro ao exportar dados: {ex.Message}')";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion Ações personalizadas
        
        private static string FormatJson(string json)
        {
            try
            {
                var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                return Newtonsoft.Json.JsonConvert.SerializeObject(parsed, Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }

        private static string GetEntityLink(string entidadeNome, string entidadeId)
        {
            try
            {
                var controller = ControllerNameHelper.GetControllerName(entidadeNome);
                return $"/{controller}/Details/{entidadeId}";
            }
            catch
            {
                return "#";
            }
        }

        private static string GenerateCSV(List<AuditLog> logs)
        {
            var csv = new System.Text.StringBuilder();

            // Header
            csv.AppendLine("Data/Hora,Usuário,Email,Operação,Entidade,ID Entidade,Campos Alterados,IP Cliente,Sucesso,Erro");

            // Dados
            foreach (var log in logs)
            {
                csv.AppendLine($"{log.DataHora:yyyy-MM-dd HH:mm:ss}," +
                              $"\"{log.UsuarioNome}\"," +
                              $"\"{log.UsuarioEmail}\"," +
                              $"\"{log.TipoOperacao.GetDescription()}\"," +
                              $"\"{log.EntidadeDisplayName ?? log.EntidadeNome}\"," +
                              $"\"{log.EntidadeId}\"," +
                              $"\"{log.CamposAlterados}\"," +
                              $"\"{log.IpCliente}\"," +
                              $"{(log.Sucesso ? "Sim" : "Não")}," +
                              $"\"{log.MensagemErro}\"");
            }

            return csv.ToString();
        }
    }
}