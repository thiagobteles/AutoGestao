namespace FGT.Models;

/// <summary>
/// ViewModel para formulários de processamento
/// </summary>
public class ProcessingFormViewModel
{
    /// <summary>
    /// Título do formulário de processamento
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// Descrição do processamento
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ícone FontAwesome
    /// </summary>
    public string Icon { get; set; } = "fas fa-cog";

    /// <summary>
    /// Nome da action para processar
    /// </summary>
    public string ActionName { get; set; } = "Process";

    /// <summary>
    /// Nome do controller
    /// </summary>
    public string ControllerName { get; set; } = "";

    /// <summary>
    /// Modelo da entidade de processamento
    /// </summary>
    public object Model { get; set; } = new();

    /// <summary>
    /// Seções do formulário (campos agrupados)
    /// </summary>
    public List<FormSectionViewModel> Sections { get; set; } = [];

    /// <summary>
    /// Erros de validação
    /// </summary>
    public Dictionary<string, string> ModelState { get; set; } = [];

    /// <summary>
    /// Texto do botão de executar
    /// </summary>
    public string SubmitButtonText { get; set; } = "Processar";

    /// <summary>
    /// Ícone do botão de executar
    /// </summary>
    public string SubmitButtonIcon { get; set; } = "fas fa-play";

    /// <summary>
    /// Exibir confirmação antes de processar
    /// </summary>
    public bool ShowConfirmation { get; set; } = true;

    /// <summary>
    /// Mensagem de confirmação
    /// </summary>
    public string? ConfirmationMessage { get; set; }

    /// <summary>
    /// Permite múltiplas execuções
    /// </summary>
    public bool AllowMultipleExecutions { get; set; } = true;

    /// <summary>
    /// URL de redirecionamento após sucesso
    /// </summary>
    public string? RedirectOnSuccessUrl { get; set; }

    /// <summary>
    /// Resultado do último processamento (se houver)
    /// </summary>
    public ProcessingResultViewModel? LastResult { get; set; }
}

/// <summary>
/// ViewModel para resultado de processamento (exibição)
/// </summary>
public class ProcessingResultViewModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = [];
}
