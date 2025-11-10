using FGT.Controllers.Base;
using FGT.Data;
using FGT.Entidades.Processing;
using FGT.Interfaces;
using FGT.Models;
using FGT.Services.Interface;

namespace FGT.Controllers
{
    /// <summary>
    /// Controller para importação de Negociações Fiscais via Excel
    /// </summary>
    //[Authorize(Roles = "Admin,Gerente,Financeiro")]
    public class ImportacaoNegociacaoFiscalController(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<ProcessingController<ImportacaoNegociacaoFiscal, ImportacaoNegociacaoFiscalResult>> logger)
        : ProcessingController<ImportacaoNegociacaoFiscal, ImportacaoNegociacaoFiscalResult>(context, fileStorageService, logger)
    {
        // O ProcessingController base já lida com tudo:
        // 1. Exibe o formulário (Index)
        // 2. Processa o arquivo enviado (Process)
        // 3. Chama automaticamente o método ProcessAsync da entidade ImportacaoNegociacaoFiscal

        protected override Task OnAfterProcessSuccessAsync(ImportacaoNegociacaoFiscal entity, ProcessingResult<ImportacaoNegociacaoFiscalResult> result)
        {
            // Após sucesso, podemos adicionar notificação customizada
            if (result.Data != null)
            {
                var detalhes = $"Total: {result.Data.TotalLinhas} | Importadas: {result.Data.LinhasImportadas} | Erros: {result.Data.LinhasComErro}";
                TempData["NotificationScript"] = $"showSuccess('Importação concluída! {detalhes}')";
            }

            return base.OnAfterProcessSuccessAsync(entity, result);
        }

        protected override Task OnAfterProcessFailureAsync(ImportacaoNegociacaoFiscal entity, ProcessingResult<ImportacaoNegociacaoFiscalResult> result)
        {
            // Após falha, podemos adicionar detalhes dos erros
            TempData["NotificationScript"] = $"showError('Erro na importação: {result.Message}')";
            return base.OnAfterProcessFailureAsync(entity, result);
        }
    }
}
