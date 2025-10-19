using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Helpers;
using AutoGestao.Models;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoGestao.Controllers.Base
{
    /// <summary>
    /// Controller para geração automática de formulários
    /// Elimina necessidade de criar views e configurações manuais
    /// </summary>
    /// <typeparam name="T">Entidade que herda de BaseEntidade</typeparam>
    public abstract class AutoFormController<T>(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<AutoGridController<T>>? logger = null) : AutoGridController<T>(context, fileStorageService, logger) where T : BaseEntidade, new()
    {

        #region Auto-Form Generation

        /// <summary>
        /// GET: Create - Formulário de criação automático
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var entity = new T();
            await SetDefaultValues(entity);

            var viewModel = await BuildAutoFormViewModelAsync(entity, "Create");

            return ShouldUseTabbedForm()
                ? View("_AutoTabbedForm", viewModel)
                : View("_AutoForm", viewModel);
        }

        /// <summary>
        /// POST: Create - Processamento automático de criação
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create([FromForm] T entity)
        {
            try
            {
                // Validações automáticas
                await ApplyAutoValidationsAsync(entity, "Create");

                if (ModelState.IsValid)
                {
                    // Auto-set audit fields
                    entity.IdEmpresa = GetCurrentEmpresaId();
                    entity.DataCadastro = DateTime.UtcNow;
                    entity.CriadoPorUsuarioId = GetCurrentUserId();

                    // Processar uploads de arquivos automaticamente
                    await ProcessAutoFileUploadsAsync(entity);

                    // Aplicar transformações automáticas
                    await ApplyAutoTransformationsAsync(entity, "Create");

                    _context.Set<T>().Add(entity);
                    await _context.SaveChangesAsync();

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Registro criado com sucesso!", redirectUrl = Url.Action("Index") });
                    }

                    TempData["Success"] = "Registro criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao criar {EntityType}", typeof(T).Name);

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = $"Erro ao criar: {ex.Message}" });
                }

                ModelState.AddModelError("", $"Erro ao criar registro: {ex.Message}");
            }

            // Se chegou aqui, houve erro - reexibir formulário
            var viewModel = await BuildAutoFormViewModelAsync(entity, "Create");
            AddModelStateToViewModel(viewModel as StandardFormViewModel);

            return ShouldUseTabbedForm()
                ? View("_AutoTabbedForm", viewModel)
                : View("_AutoForm", viewModel);
        }

        /// <summary>
        /// GET: Edit - Formulário de edição automático
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Edit(long id)
        {
            var entity = await GetEntityWithIncludesAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var viewModel = await BuildAutoFormViewModelAsync(entity, "Edit");

            return ShouldUseTabbedForm()
                ? View("_AutoTabbedForm", viewModel)
                : View("_AutoForm", viewModel);
        }

        /// <summary>
        /// POST: Edit - Processamento automático de edição
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(long id, [FromForm] T entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            try
            {
                // Validações automáticas
                await ApplyAutoValidationsAsync(entity, "Edit");

                if (ModelState.IsValid)
                {
                    var existingEntity = await _context.Set<T>().FindAsync(id);
                    if (existingEntity == null)
                    {
                        return NotFound();
                    }

                    // Auto-update audit fields
                    entity.DataAlteracao = DateTime.UtcNow;
                    entity.AlteradoPorUsuarioId = GetCurrentUserId();

                    // Processar uploads de arquivos automaticamente
                    await ProcessAutoFileUploadsAsync(entity);

                    // Aplicar transformações automáticas
                    await ApplyAutoTransformationsAsync(entity, "Edit");

                    // Auto-update apenas campos modificados
                    await UpdateEntityPropertiesAsync(existingEntity, entity);

                    await _context.SaveChangesAsync();

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Registro atualizado com sucesso!", redirectUrl = Url.Action("Index") });
                    }

                    TempData["Success"] = "Registro atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao editar {EntityType} ID {Id}", typeof(T).Name, id);

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = $"Erro ao atualizar: {ex.Message}" });
                }

                ModelState.AddModelError("", $"Erro ao atualizar registro: {ex.Message}");
            }

            // Se chegou aqui, houve erro - reexibir formulário
            var viewModel = await BuildAutoFormViewModelAsync(entity, "Edit");
            AddModelStateToViewModel(viewModel as StandardFormViewModel);

            return ShouldUseTabbedForm()
                ? View("_AutoTabbedForm", viewModel)
                : View("_AutoForm", viewModel);
        }

        /// <summary>
        /// GET: Details - Visualização automática
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Details(long id)
        {
            var entity = await GetEntityWithIncludesAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var viewModel = await BuildAutoFormViewModelAsync(entity, "Details");

            return ShouldUseTabbedForm()
                ? View("_AutoTabbedForm", viewModel)
                : View("_AutoForm", viewModel);
        }

        /// <summary>
        /// DELETE: Delete - Exclusão automática
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Delete(long id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }

                // Verificar dependências antes de excluir
                var hasDependencies = await CheckDependenciesAsync(entity);
                if (hasDependencies.HasDependencies)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = false, message = hasDependencies.Message });
                    }

                    TempData["Error"] = hasDependencies.Message;
                    return RedirectToAction(nameof(Index));
                }

                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = true, message = "Registro excluído com sucesso!" });
                }

                TempData["Success"] = "Registro excluído com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao excluir {EntityType} ID {Id}", typeof(T).Name, id);

                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = $"Erro ao excluir: {ex.Message}" });
                }

                TempData["Error"] = $"Erro ao excluir registro: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Auto-Form Building

        /// <summary>
        /// Constrói ViewModel do formulário automaticamente
        /// </summary>
        protected virtual async Task<object> BuildAutoFormViewModelAsync(T entity, string action)
        {
            return ShouldUseTabbedForm()
                ? await BuildAutoTabbedFormViewModelAsync(entity, action)
                : (object)await BuildAutoStandardFormViewModelAsync(entity, action);
        }

        /// <summary>
        /// Constrói formulário padrão automático
        /// </summary>
        protected virtual async Task<StandardFormViewModel> BuildAutoStandardFormViewModelAsync(T entity, string action)
        {
            var formConfig = _metadata.FormConfig ?? new FormConfigAttribute();
            var sections = await BuildAutoSectionsAsync(entity, action);

            var viewModel = new StandardFormViewModel
            {
                Title = formConfig.Title ?? AutoFormController<T>.GetAutoTitle(action),
                Subtitle = formConfig.Subtitle ?? AutoFormController<T>.GetAutoSubtitle(action),
                Icon = formConfig.Icon ?? GetConventionBasedIcon(),
                BackAction = formConfig.BackAction ?? "Index",
                BackText = formConfig.BackText ?? "Voltar à Lista",
                ActionName = action,
                ControllerName = GetControllerName(),
                Model = entity,
                EnableAjaxSubmit = formConfig.EnableAjaxSubmit,
                IsEditMode = action == "Edit",
                IsDetailsMode = action == "Details",
                Sections = sections
            };

            return viewModel;
        }

        /// <summary>
        /// Constrói formulário com abas automático
        /// </summary>
        protected virtual async Task<TabbedFormViewModel> BuildAutoTabbedFormViewModelAsync(T entity, string action)
        {
            var formConfig = _metadata.FormConfig ?? new FormConfigAttribute();
            var formTabs = _metadata.FormTabs ?? new FormTabsAttribute();
            var sections = await BuildAutoSectionsAsync(entity, action);
            var tabs = BuildAutoTabs(entity);

            var viewModel = new TabbedFormViewModel
            {
                Title = formConfig.Title ?? AutoFormController<T>.GetAutoTitle(action),
                Subtitle = formConfig.Subtitle ?? AutoFormController<T>.GetAutoSubtitle(action),
                Icon = formConfig.Icon ?? GetConventionBasedIcon(),
                BackAction = formConfig.BackAction ?? "Index",
                BackText = formConfig.BackText ?? "Voltar à Lista",
                ActionName = action,
                ControllerName = GetControllerName(),
                Model = entity,
                EnableAjaxSubmit = formConfig.EnableAjaxSubmit,
                IsEditMode = action == "Edit",
                IsDetailsMode = action == "Details",
                EnableTabs = formTabs.EnableTabs,
                EntityId = entity.Id,
                ActiveTab = formTabs.DefaultTab ?? "principal",
                Tabs = tabs,
                Sections = sections
            };

            return viewModel;
        }

        /// <summary>
        /// Constrói seções automaticamente
        /// </summary>
        protected virtual async Task<List<FormSectionViewModel>> BuildAutoSectionsAsync(T entity, string action)
        {
            var sections = new List<FormSectionViewModel>();
            var properties = _metadata.FormProperties;

            // Agrupar por seção
            var fieldsBySection = new Dictionary<string, List<FormFieldViewModel>>();

            foreach (var property in properties)
            {
                var field = await BuildAutoFieldAsync(property, entity, action);
                if (field == null)
                {
                    continue;
                }

                var sectionName = field.Section ?? "Gerais";
                if (!fieldsBySection.ContainsKey(sectionName))
                {
                    fieldsBySection[sectionName] = [];
                }

                fieldsBySection[sectionName].Add(field);
            }

            // Criar seções
            foreach (var sectionGroup in fieldsBySection.OrderBy(g => PropertyExtensions.GetSectionOrder(g.Key)))
            {
                var section = new FormSectionViewModel
                {
                    Name = sectionGroup.Key,
                    Icon = PropertyExtensions.GetSectionIcon(sectionGroup.Key),
                    Fields = [.. sectionGroup.Value.OrderBy(f => f.Order)]
                };

                section.GridColumns = section.Fields.Count != 0 ? section.Fields.Max(f => f.GridColumns) : 1;
                sections.Add(section);
            }

            return sections;
        }

        /// <summary>
        /// Constrói campo automaticamente
        /// </summary>
        protected virtual async Task<FormFieldViewModel?> BuildAutoFieldAsync(PropertyInfo property, T entity, string action)
        {
            var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();
            if (formFieldAttr == null)
            {
                return null;
            }

            // Verificar condições de exibição
            if (!ShouldDisplayField(property, entity, action))
            {
                return null;
            }

            var field = new FormFieldViewModel
            {
                PropertyName = property.Name,
                DisplayName = formFieldAttr.Name ?? property.GetDisplayName() ?? property.Name,
                Icon = formFieldAttr.Icon ?? PropertyExtensions.GetDefaultIcon(property),
                Placeholder = formFieldAttr.Placeholder ?? PropertyExtensions.GetDefaultPlaceholder(property),
                Type = formFieldAttr.Type,
                Required = await GetAutoRequired(property, entity, action),
                ReadOnly = action == "Details" || formFieldAttr.ReadOnly,
                Value = GetAutoValue(property, entity),
                ValidationRegex = formFieldAttr.ValidationRegex,
                ValidationMessage = formFieldAttr.ValidationMessage,
                GridColumns = formFieldAttr.GridColumns,
                CssClass = formFieldAttr.CssClass,
                Order = formFieldAttr.Order,
                Section = formFieldAttr.Section
            };

            // Configurações específicas por tipo
            await ConfigureFieldByTypeAsync(field, property, entity, formFieldAttr);

            return field;
        }

        #endregion

        #region Auto-Validations & Transformations

        /// <summary>
        /// Aplica validações automáticas
        /// </summary>
        protected virtual async Task ApplyAutoValidationsAsync(T entity, string action)
        {
            // Validações por atributo
            await ApplyAttributeValidationsAsync(entity);

            // Validações por convenção
            await ApplyConventionValidationsAsync(entity);

            // Validações de negócio customizadas
            await ApplyCustomValidationsAsync(entity, action);
        }

        /// <summary>
        /// Aplica transformações automáticas
        /// </summary>
        protected virtual async Task ApplyAutoTransformationsAsync(T entity, string action)
        {
            // Transformações de formato
            ApplyFormatTransformations(entity);

            // Transformações de negócio
            await ApplyBusinessTransformationsAsync(entity, action);
        }

        /// <summary>
        /// Processa uploads de arquivos automaticamente
        /// </summary>
        protected virtual async Task ProcessAutoFileUploadsAsync(T entity)
        {
            var fileProperties = _metadata.AllProperties
                .Where(p => p.GetCustomAttribute<FormFieldAttribute>()?.Type is EnumFieldType.File or EnumFieldType.Image)
                .ToList();

            foreach (var property in fileProperties)
            {
                var fileKey = property.Name;
                if (Request.Form.Files.Any(f => f.Name == fileKey))
                {
                    var file = Request.Form.Files[fileKey];
                    if (file != null && file.Length > 0)
                    {
                        var filePath = await _fileStorageService.SaveFileAsync(
                            file,
                            typeof(T).Name,
                            GetCurrentEmpresaId());

                        property.SetValue(entity, filePath);
                    }
                }
            }
        }

        #endregion

        #region Utility Methods

        private bool ShouldUseTabbedForm()
        {
            return _metadata.HasTabs;
        }

        private static string GetAutoTitle(string action)
        {
            var entityName = typeof(T).Name;
            return action switch
            {
                "Create" => $"Novo {entityName}",
                "Edit" => $"Editar {entityName}",
                "Details" => $"Detalhes do {entityName}",
                _ => entityName
            };
        }

        private static string GetAutoSubtitle(string action)
        {
            var entityName = typeof(T).Name.ToLower();
            return action switch
            {
                "Create" => $"Criar novo registro de {entityName}",
                "Edit" => $"Alterar dados do {entityName}",
                "Details" => $"Visualizar informações do {entityName}",
                _ => $"Gerenciar {entityName}"
            };
        }

        private List<FormTabViewModel> BuildAutoTabs(T entity)
        {
            var tabs = new List<FormTabViewModel>
            {
                new()
                {
                    TabId = "principal",
                    TabName = "Dados Principais",
                    TabIcon = "fas fa-info-circle",
                    Order = 0,
                    LazyLoad = false,
                    IsActive = true
                }
            };

            var tabAttributes = _metadata.GetTabs();
            foreach (var tabAttr in tabAttributes)
            {
                tabs.Add(new FormTabViewModel
                {
                    TabId = tabAttr.TabId,
                    TabName = tabAttr.TabName,
                    TabIcon = tabAttr.TabIcon,
                    Order = tabAttr.Order,
                    Controller = tabAttr.Controller,
                    Action = tabAttr.Action,
                    LazyLoad = tabAttr.LazyLoad,
                    HasAccess = HasTabAccess(tabAttr.RequiredRoles),
                    Parameters = new Dictionary<string, object>
                    {
                        ["id"] = entity.Id,
                        ["entityType"] = typeof(T).Name
                    }
                });
            }

            return tabs.OrderBy(t => t.Order).ToList();
        }

        protected virtual bool ShouldDisplayField(PropertyInfo property, T entity, string action)
        {
            var conditionalDisplay = property.GetCustomAttribute<ConditionalDisplayAttribute>();
            if (conditionalDisplay == null)
            {
                return true;
            }

            return ConditionalExpressionEvaluator.Evaluate(conditionalDisplay.Rule, entity, typeof(T));
        }

        protected virtual async Task<bool> GetAutoRequired(PropertyInfo property, T entity, string action)
        {
            var formField = property.GetCustomAttribute<FormFieldAttribute>();
            if (formField?.Required == true)
            {
                return true;
            }

            var conditionalRequired = property.GetCustomAttribute<ConditionalRequiredAttribute>();
            if (conditionalRequired != null)
            {
                return ConditionalExpressionEvaluator.Evaluate(conditionalRequired.Rule, entity, typeof(T));
            }

            return false;
        }

        protected virtual string? GetAutoValue(PropertyInfo property, T entity)
        {
            var rawValue = property.GetValue(entity);
            if (rawValue == null)
            {
                return null;
            }

            var formField = property.GetCustomAttribute<FormFieldAttribute>();
            return FormatFieldValueToString(rawValue, formField?.Type ?? EnumFieldType.Text, property);
        }

        protected virtual async Task ConfigureFieldByTypeAsync(FormFieldViewModel field, PropertyInfo property, T entity, FormFieldAttribute formFieldAttr)
        {
            switch (field.Type)
            {
                case EnumFieldType.Select when property.PropertyType.IsEnum:
                    field.Options = EnumAutomationHelper.GetEnumSelectListItems(property.Name, typeof(T), true);
                    break;

                case EnumFieldType.Reference:
                    await ConfigureReferenceFieldAsync(field, property, formFieldAttr);
                    break;

                case EnumFieldType.File or EnumFieldType.Image:
                    await ConfigureFileFieldAsync(field, property, entity);
                    break;
            }
        }

        private async Task ConfigureReferenceFieldAsync(FormFieldViewModel field, PropertyInfo property, FormFieldAttribute formFieldAttr)
        {
            if (formFieldAttr.Reference == null)
            {
                return;
            }

            field.Reference = formFieldAttr.Reference;
            field.ReferenceConfig = ReferenceFieldConfig.GetDefault(formFieldAttr.Reference);

            // Auto-configure based on conventions
            field.ReferenceConfig.ControllerName = ControllerNameHelper.GetControllerName(formFieldAttr.Reference);
            field.ReferenceConfig.DisplayField = GetReferenceDisplayField(formFieldAttr.Reference);
            field.ReferenceConfig.SearchFields = GetReferenceSearchFields(formFieldAttr.Reference);
        }

        private async Task ConfigureFileFieldAsync(FormFieldViewModel field, PropertyInfo property, T entity)
        {
            var rawValue = property.GetValue(entity);
            if (rawValue != null)
            {
                var filePath = rawValue.ToString();
                if (!string.IsNullOrEmpty(filePath))
                {
                    field.FilePath = filePath;
                    field.FileName = Path.GetFileName(filePath);

                    try
                    {
                        field.FileUrl = await _fileStorageService.GetDownloadUrlAsync(
                            filePath,
                            typeof(T).Name,
                            GetCurrentEmpresaId());
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Erro ao gerar URL do arquivo: {FileName}", filePath);
                    }
                }
            }
        }

        // Override methods for customization
        protected virtual async Task SetDefaultValues(T entity) { }
        protected virtual async Task ApplyAttributeValidationsAsync(T entity) { }
        protected virtual async Task ApplyConventionValidationsAsync(T entity) { }
        protected virtual async Task ApplyCustomValidationsAsync(T entity, string action) { }
        protected virtual void ApplyFormatTransformations(T entity) { }
        protected virtual async Task ApplyBusinessTransformationsAsync(T entity, string action) { }
        protected virtual async Task<T?> GetEntityWithIncludesAsync(long id)
        {
            return await _context.Set<T>()
                .ApplyAutoIncludes(_metadata)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        protected virtual async Task UpdateEntityPropertiesAsync(T existingEntity, T newEntity)
        {
            _context.Entry(existingEntity).CurrentValues.SetValues(newEntity);
        }
        protected virtual async Task<(bool HasDependencies, string Message)> CheckDependenciesAsync(T entity)
        {
            return (false, "");
        }

        #endregion

        #region Métodos de Apoio Implementados

        /// <summary>
        /// Obtém ID do usuário atual
        /// </summary>
        protected virtual long GetCurrentUserId()
        {
            // TODO: Implementar obtenção do usuário atual do contexto/sessão
            // Por enquanto retorna 1 como fallback
            try
            {
                // Tentar obter do HttpContext se disponível
                if (HttpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = HttpContext.User.FindFirst("UserId") ??
                                     HttpContext.User.FindFirst("sub") ??
                                     HttpContext.User.FindFirst("id");

                    if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
                    {
                        return userId;
                    }
                }

                // Fallback para desenvolvimento
                return 1;
            }
            catch
            {
                return 1; // Fallback seguro
            }
        }

        /// <summary>
        /// Obtém campo de exibição para referência
        /// </summary>
        protected virtual string GetReferenceDisplayField(Type referenceType)
        {
            if (referenceType == null)
            {
                return "Id";
            }

            var properties = referenceType.GetProperties();

            // Procurar propriedades comuns para display
            var displayProperty = properties.FirstOrDefault(p =>
                p.Name.Equals("Nome", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Descricao", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Title", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Name", StringComparison.OrdinalIgnoreCase));

            return displayProperty?.Name ?? "Id";
        }

        /// <summary>
        /// Verifica se usuário tem acesso à tab
        /// </summary>
        protected virtual bool HasTabAccess(string[]? requiredRoles)
        {
            if (requiredRoles == null || requiredRoles.Length == 0)
            {
                return true;
            }

            try
            {
                // TODO: Implementar verificação de roles/permissões
                // Por enquanto retorna true para desenvolvimento
                if (HttpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    // Verificar se usuário tem alguma das roles necessárias
                    return requiredRoles.Any(role => HttpContext.User.IsInRole(role));
                }

                // Se não autenticado, permitir acesso para desenvolvimento
                return true;
            }
            catch
            {
                return true; // Fallback permissivo para desenvolvimento
            }
        }

        /// <summary>
        /// Obtém campos de busca para referência
        /// </summary>
        protected virtual List<string> GetReferenceSearchFields(Type referenceType)
        {
            if (referenceType == null)
            {
                return ["Id"];
            }

            var properties = referenceType.GetProperties();
            var searchFields = new List<string>();

            // Adicionar campos comuns de busca
            var commonSearchFields = new[] { "Nome", "Descricao", "Title", "Codigo", "Name" };

            foreach (var fieldName in commonSearchFields)
            {
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

                if (property != null && property.PropertyType == typeof(string))
                {
                    searchFields.Add(property.Name);
                }
            }

            return searchFields.Any() ? searchFields : ["Id"];
        }

        /// <summary>
        /// Formata valor do campo para string
        /// </summary>
        protected virtual string FormatFieldValueToString(object? value, EnumFieldType fieldType, PropertyInfo property)
        {
            if (value == null)
            {
                return "";
            }

            try
            {
                return fieldType switch
                {
                    EnumFieldType.Date when value is DateTime dateTime =>
                        dateTime.ToString("dd/MM/yyyy"),

                    EnumFieldType.DateTime when value is DateTime dateTimeWithTime =>
                        dateTimeWithTime.ToString("dd/MM/yyyy HH:mm"),

                    EnumFieldType.Number when value is decimal decimalValue =>
                        decimalValue.ToString("N2"),

                    EnumFieldType.Currency when value is decimal moneyValue =>
                        moneyValue.ToString("C2"),

                    EnumFieldType.Percentage when value is decimal percentValue =>
                        percentValue.ToString("P2"),

                    EnumFieldType.Checkbox when value is bool boolValue =>
                        boolValue ? "Sim" : "Não",

                    EnumFieldType.Select when property.PropertyType.IsEnum =>
                        Convert.ToInt32(value).ToString(),

                EnumFieldType.Reference =>
                        GetReferenceDisplayValue(value),

                    _ => value.ToString() ?? ""
                };
            }
            catch
            {
                return value.ToString() ?? "";
            }
        }

        /// <summary>
        /// Obtém valor de exibição para referência
        /// </summary>
        protected virtual string GetReferenceDisplayValue(object referenceValue)
        {
            if (referenceValue == null)
            {
                return "";
            }

            try
            {
                var type = referenceValue.GetType();
                var displayField = GetReferenceDisplayField(type);
                var property = type.GetProperty(displayField);

                if (property != null)
                {
                    var value = property.GetValue(referenceValue);
                    return value?.ToString() ?? "";
                }

                return referenceValue.ToString() ?? "";
            }
            catch
            {
                return referenceValue.ToString() ?? "";
            }
        }

        /// <summary>
        /// Adiciona erros do ModelState ao ViewModel
        /// </summary>
        protected virtual void AddModelStateToViewModel(StandardFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }

            try
            {
                viewModel.ModelState = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => string.Join("; ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                    );
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao adicionar ModelState ao ViewModel");
            }
        }

        /// <summary>
        /// Adiciona erros do ModelState ao ViewModel com abas
        /// </summary>
        protected virtual void AddModelStateToViewModel(TabbedFormViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }

            try
            {
                viewModel.ModelState = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => string.Join("; ", kvp.Value.Errors.Select(e => e.ErrorMessage))
                    );
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao adicionar ModelState ao ViewModel com abas");
            }
        }

        #endregion
    }
}