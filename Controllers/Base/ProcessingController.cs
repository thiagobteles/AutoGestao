using FGT.Atributes;
using FGT.Data;
using FGT.Extensions;
using FGT.Helpers;
using FGT.Interfaces;
using FGT.Models;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FGT.Controllers.Base
{
    /// <summary>
    /// Controller base para telas de processamento genéricas
    /// Herdar desta classe para criar telas de processamento automáticas
    /// </summary>
    /// <typeparam name="T">Tipo da entidade de processamento que implementa IProcessingEntity</typeparam>
    /// <typeparam name="TResult">Tipo do resultado do processamento</typeparam>
    public abstract class ProcessingController<T, TResult>(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<ProcessingController<T, TResult>>? logger = null)
        : Controller where T : class, IProcessingEntity<TResult>, new()
    {
        protected readonly ApplicationDbContext _context = context;
        protected readonly IFileStorageService _fileStorageService = fileStorageService;
        protected readonly ILogger<ProcessingController<T, TResult>>? _logger = logger;

        protected ProcessingController(ApplicationDbContext context, IFileStorageService fileStorageService) : this(context, fileStorageService, null)
        {
        }

        #region Métodos Protegidos Virtuais (podem ser sobrescritos)

        /// <summary>
        /// Permite customizar o ViewModel antes de exibir o formulário
        /// </summary>
        protected virtual ProcessingFormViewModel CustomizeViewModel(ProcessingFormViewModel viewModel)
        {
            return viewModel;
        }

        /// <summary>
        /// Validação customizada antes de executar o processamento
        /// Retorna null se válido, ou mensagem de erro se inválido
        /// </summary>
        protected virtual Task<string?> ValidateBeforeProcessAsync(T entity)
        {
            return Task.FromResult<string?>(null);
        }

        /// <summary>
        /// Hook executado antes do processamento (pode modificar a entidade)
        /// </summary>
        protected virtual Task OnBeforeProcessAsync(T entity)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook executado após o processamento bem-sucedido
        /// </summary>
        protected virtual Task OnAfterProcessSuccessAsync(T entity, ProcessingResult<TResult> result)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook executado após falha no processamento
        /// </summary>
        protected virtual Task OnAfterProcessFailureAsync(T entity, ProcessingResult<TResult> result)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Obtém o ID da empresa atual do usuário logado
        /// </summary>
        protected virtual long GetCurrentEmpresaId()
        {
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            return long.TryParse(empresaIdClaim, out var empresaId)
                ? empresaId
                : throw new UnauthorizedAccessException("Usuário não possui empresa associada");
        }

        /// <summary>
        /// Obtém o ID do usuário logado
        /// </summary>
        protected virtual long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return long.TryParse(userIdClaim, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Usuário não autenticado");
        }

        /// <summary>
        /// Obtém o ID da empresa cliente do usuário (se houver)
        /// </summary>
        protected virtual long? GetCurrentEmpresaClienteId()
        {
            var empresaClienteIdClaim = User.FindFirst("EmpresaClienteId")?.Value;
            return long.TryParse(empresaClienteIdClaim, out var empresaClienteId)
                ? empresaClienteId
                : null;
        }

        #endregion

        #region Actions Públicas

        /// <summary>
        /// GET: Exibe o formulário de processamento
        /// </summary>
        [HttpGet]
        public virtual IActionResult Index()
        {
            var viewModel = BuildViewModel();
            viewModel = CustomizeViewModel(viewModel);

            // Adicionar flag para limpar lock de processamento no cliente
            ViewBag.ClearProcessingLock = true;

            return View("~/Views/Shared/_StandardProcessing.cshtml", viewModel);
        }

        /// <summary>
        /// POST: Executa o processamento
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Process([FromForm] T model)
        {
            _logger?.LogInformation("🚀 Iniciando processamento para {EntityType}", typeof(T).Name);

            try
            {
                // 1. Validação customizada
                var validationError = await ValidateBeforeProcessAsync(model);
                if (!string.IsNullOrEmpty(validationError))
                {
                    _logger?.LogWarning("❌ Validação customizada falhou: {Error}", validationError);
                    return Json(new
                    {
                        success = false,
                        message = validationError
                    });
                }

                // 2. 🔒 FORÇAR EMPRESACLIENTEID PARA USUÁRIOS NÃO-ADMIN (LÓGICA CENTRALIZADA)
                EmpresaClienteFieldHelper.ForceEmpresaClienteId(model, User);
                _logger?.LogInformation("🔒 EmpresaClienteId validado/forçado para processamento");

                // 3. Processar upload de arquivos (se houver)
                await ProcessFileUploadsAsync(model);

                // 4. Hook antes do processamento
                await OnBeforeProcessAsync(model);

                // 5. Executar processamento
                var userId = GetCurrentUserId();
                var empresaId = GetCurrentEmpresaId();

                _context.CurrentEmpresaId = empresaId;

                var result = await model.ProcessAsync(_context, _fileStorageService, userId, empresaId);

                // 6. Log do resultado
                if (result.Success)
                {
                    _logger?.LogInformation("✅ Processamento concluído com sucesso: {Message}", result.Message);
                    await OnAfterProcessSuccessAsync(model, result);
                }
                else
                {
                    _logger?.LogWarning("❌ Processamento falhou: {Message}", result.Message);
                    await OnAfterProcessFailureAsync(model, result);
                }

                // 7. Retornar resultado
                //var retorno = Json(new
                //{
                //    success = result.Success,
                //    message = result.Message,
                //    errors = result.Errors,
                //    warnings = result.Warnings,
                //    metadata = result.Metadata
                //});

                TempData["NotificationScript"] = $"showSuccess('{result.Message}')";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "💥 Erro ao executar processamento: {Message}", ex.Message);
                TempData["NotificationScript"] = $"showError('Erro ao executar processamento: {ex.Message}')";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Constrói o ViewModel do formulário de processamento
        /// </summary>
        private ProcessingFormViewModel BuildViewModel(ProcessingResultViewModel? lastResult = null)
        {
            var processingFormAttr = typeof(T).GetCustomAttribute<ProcessingFormAttribute>();
            var properties = typeof(T).GetProperties();

            // Agrupar campos por seção
            var sections = new Dictionary<string, List<FormFieldViewModel>>();

            foreach (var prop in properties)
            {
                var formFieldAttr = prop.GetCustomAttribute<FormFieldAttribute>();
                if (formFieldAttr == null)
                {
                    continue;
                }

                var sectionName = formFieldAttr.Section ?? "Geral";
                if (!sections.TryGetValue(sectionName, out var value))
                {
                    value = [];
                    sections[sectionName] = value;
                }

                // 🔒 LÓGICA CENTRALIZADA DE EMPRESA CLIENTE
                bool shouldHideField = false;
                object? defaultValue = null;

                if (EmpresaClienteFieldHelper.ShouldHideEmpresaClienteField(prop, User, out var empresaClienteIdLogada))
                {
                    shouldHideField = true;
                    defaultValue = empresaClienteIdLogada.Value;
                    _logger?.LogInformation("🔒 Campo {PropertyName} oculto para usuário não-admin. EmpresaClienteId forçado: {Value}", prop.Name, empresaClienteIdLogada);
                }

                // Determinar o placeholder
                var placeholder = formFieldAttr.Placeholder;
                if (string.IsNullOrEmpty(placeholder) && formFieldAttr.Type == Enumerador.Gerais.EnumFieldType.Reference && formFieldAttr.Reference != null)
                {
                    // Gerar placeholder automaticamente baseado nos campos ReferenceSearchable
                    placeholder = FormFieldViewModelExtensions.GetReferencePlaceholder(formFieldAttr.Reference);
                }

                var fieldViewModel = new FormFieldViewModel
                {
                    PropertyName = prop.Name,
                    DisplayName = formFieldAttr.Name,
                    Type = shouldHideField ? Enumerador.Gerais.EnumFieldType.Hidden : formFieldAttr.Type,
                    Required = formFieldAttr.Required,
                    Icon = formFieldAttr.Icon,
                    GridColumns = formFieldAttr.GridColumns,
                    Order = formFieldAttr.Order,
                    Reference = formFieldAttr.Reference,
                    AllowedExtensions = formFieldAttr.AllowedExtensions,
                    MaxSizeMB = formFieldAttr.MaxSizeMB,
                    ImageSize = formFieldAttr.ImageSize,
                    Placeholder = placeholder,
                    HelpText = formFieldAttr.HelpText,
                    Value = defaultValue ?? prop.GetValue(new T()),
                    Options = formFieldAttr.Type == Enumerador.Gerais.EnumFieldType.Select ? GetAutoEnumOptions(prop) : [],
                    ReferenceConfig = formFieldAttr.Reference != null ? ReferenceFieldConfig.GetDefault(formFieldAttr.Reference) : new(),
                    ReadOnly = shouldHideField
                };
                value.Add(fieldViewModel);
            }

            // Criar seções ordenadas
            var sectionViewModels = sections
                .Select(kvp => new FormSectionViewModel
                {
                    Name = kvp.Key,
                    Fields = [.. kvp.Value.OrderBy(f => f.Order)]
                })
                .ToList();

            var viewModel = new ProcessingFormViewModel
            {
                Title = processingFormAttr?.Title ?? typeof(T).Name,
                Description = processingFormAttr?.Description,
                Icon = processingFormAttr?.Icon ?? "fas fa-cog",
                ActionName = "Process",
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                Model = new T(),
                Sections = sectionViewModels,
                SubmitButtonText = processingFormAttr?.SubmitButtonText ?? "Processar",
                SubmitButtonIcon = processingFormAttr?.SubmitButtonIcon ?? "fas fa-play",
                ShowConfirmation = processingFormAttr?.ShowConfirmation ?? true,
                ConfirmationMessage = processingFormAttr?.ConfirmationMessage,
                AllowMultipleExecutions = processingFormAttr?.AllowMultipleExecutions ?? true,
                RedirectOnSuccessUrl = processingFormAttr?.RedirectOnSuccessUrl,
                LastResult = lastResult
            };

            return viewModel;
        }

        /// <summary>
        /// Processa upload de arquivos nos campos do tipo File/Image
        /// </summary>
        private async Task ProcessFileUploadsAsync(T model)
        {
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var formFieldAttr = prop.GetCustomAttribute<FormFieldAttribute>();
                if (formFieldAttr == null)
                {
                    continue;
                }

                // Verificar se é campo de arquivo
                if (formFieldAttr.Type != Enumerador.Gerais.EnumFieldType.File && formFieldAttr.Type != Enumerador.Gerais.EnumFieldType.Image)
                {
                    continue;
                }

                // Verificar se há arquivo no request
                var file = Request.Form.Files.GetFile(prop.Name);
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                _logger?.LogInformation("📁 Processando upload do arquivo: {FileName} (Campo: {PropertyName})", file.FileName, prop.Name);

                // Validar extensão
                if (!string.IsNullOrEmpty(formFieldAttr.AllowedExtensions))
                {
                    var extension = Path.GetExtension(file.FileName).TrimStart('.');
                    var allowedExtensions = formFieldAttr.AllowedExtensions.Split(',').Select(e => e.Trim().ToLowerInvariant());
                    if (!allowedExtensions.Contains(extension.ToLowerInvariant()))
                    {
                        throw new InvalidOperationException(
                            $"Extensão '{extension}' não permitida para o campo '{formFieldAttr.Name}'. " +
                            $"Extensões permitidas: {formFieldAttr.AllowedExtensions}");
                    }
                }

                // Validar tamanho
                if (formFieldAttr.MaxSizeMB > 0)
                {
                    var maxSizeBytes = formFieldAttr.MaxSizeMB * 1024 * 1024;
                    if (file.Length > maxSizeBytes)
                    {
                        throw new InvalidOperationException(
                            $"Arquivo muito grande para o campo '{formFieldAttr.Name}'. " +
                            $"Tamanho máximo: {formFieldAttr.MaxSizeMB}MB");
                    }
                }

                // Upload para MinIO
                var entityName = typeof(T).Name;
                var idEmpresa = GetCurrentEmpresaId();
                var filePath = await _fileStorageService.UploadFileAsync(file, entityName, prop.Name, idEmpresa);

                // Atualizar propriedade
                prop.SetValue(model, filePath);
                _logger?.LogInformation("✅ Arquivo uploaded com sucesso: {FilePath}", filePath);
            }
        }

        /// <summary>
        /// Obtém as opções de select automaticamente para propriedades Enum
        /// </summary>
        private static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetAutoEnumOptions(PropertyInfo property)
        {
            try
            {
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (!propertyType.IsEnum)
                {
                    return [];
                }

                // Usa reflexão para chamar EnumExtension.GetSelectListItems<TEnum>(true)
                var enumExtensionMethod = typeof(EnumExtension).GetMethod("GetSelectListItems");
                if (enumExtensionMethod == null)
                {
                    return [];
                }

                var genericMethod = enumExtensionMethod.MakeGenericMethod(propertyType);

                // Chama o método com obterIcone = true
                var result = genericMethod.Invoke(null, [true]) as List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>;
                return result ?? [];
            }
            catch
            {
                return [];
            }
        }

        #endregion
    }
}