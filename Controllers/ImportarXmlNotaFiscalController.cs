using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Entidades.Processing;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    /// <summary>
    /// Controller para importação de XML de Nota Fiscal
    /// Exemplo de uso do ProcessingController genérico
    /// ZERO código necessário - tudo é herdado!
    /// </summary>
    [Authorize]
    public class ImportarXmlNotaFiscalController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<ProcessingController<ImportarXmlNotaFiscal, NotaFiscal>> logger)
        : ProcessingController<ImportarXmlNotaFiscal, NotaFiscal>(context, fileStorageService, logger)
    {
        // Exemplo de validação customizada (opcional)
        protected override async Task<string?> ValidateBeforeProcessAsync(ImportarXmlNotaFiscal entity)
        {
            // Verificar se a empresa existe
            var empresa = await _context.EmpresasClientes.FirstOrDefaultAsync(e => e.Id == entity.EmpresaClienteId);
            if (empresa == null)
            {
                return "Empresa selecionada não encontrada";
            }

            // Validação passou
            return null;
        }

        // Exemplo de hook após sucesso (opcional)
        protected override async Task OnAfterProcessSuccessAsync(ImportarXmlNotaFiscal entity, ProcessingResult<NotaFiscal> result)
        {
            _logger?.LogInformation( "✅ Nota fiscal #{Numero} importada com sucesso pelo usuário {UserId}", result.Data?.Numero, GetCurrentUserId());
            await Task.CompletedTask;
        }

        // Exemplo de hook após falha (opcional)
        protected override async Task OnAfterProcessFailureAsync(ImportarXmlNotaFiscal entity, ProcessingResult<NotaFiscal> result)
        {
            _logger?.LogWarning("❌ Falha ao importar nota fiscal: {Message}", result.Message);
            await Task.CompletedTask;
        }
    }
}