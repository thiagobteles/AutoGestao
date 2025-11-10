namespace FGT.Atributes;

/// <summary>
/// Atributo para configurar formulários de processamento
/// Usado em classes que implementam IProcessingEntity
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ProcessingFormAttribute : Attribute
{
    /// <summary>
    /// Título do formulário de processamento
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Ícone FontAwesome (ex: "fas fa-file-import")
    /// </summary>
    public string Icon { get; set; } = "fas fa-cog";

    /// <summary>
    /// Descrição do processamento (exibida abaixo do título)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Texto do botão de executar (padrão: "Processar")
    /// </summary>
    public string SubmitButtonText { get; set; } = "Processar";

    /// <summary>
    /// Ícone do botão de executar (padrão: "fas fa-play")
    /// </summary>
    public string SubmitButtonIcon { get; set; } = "fas fa-play";

    /// <summary>
    /// Exibir modal de confirmação antes de processar
    /// </summary>
    public bool ShowConfirmation { get; set; } = true;

    /// <summary>
    /// Mensagem de confirmação (se ShowConfirmation = true)
    /// </summary>
    public string? ConfirmationMessage { get; set; }

    /// <summary>
    /// Permite executar o processamento múltiplas vezes sem recarregar a página
    /// </summary>
    public bool AllowMultipleExecutions { get; set; } = true;

    /// <summary>
    /// URL de redirecionamento após sucesso (null = permanece na página)
    /// </summary>
    public string? RedirectOnSuccessUrl { get; set; }
}
