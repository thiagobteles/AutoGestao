using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Helpers;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Models.Report;
using AutoGestao.Services;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoGestao.Controllers.Base
{
    public abstract class StandardGridController<T>(ApplicationDbContext context, IFileStorageService fileStorageService, IReportService reportService, ILogger<StandardGridController<T>>? logger = null)
        : Controller where T : BaseEntidade, new()
    {
        protected readonly ApplicationDbContext _context = context;
        protected readonly IFileStorageService _fileStorageService = fileStorageService;
        protected readonly ILogger<StandardGridController<T>>? _logger = logger;
        protected readonly IReportService _reportService = reportService;

        protected StandardGridController(ApplicationDbContext context, IFileStorageService fileStorageService, IReportService reportService) : this(context, fileStorageService, reportService, null)
        {
        }

        #region Métodos protected virtual (podem ser sobrescritos)

        protected virtual StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel standardGridViewModel)
        {
            return standardGridViewModel;
        }

        protected virtual IQueryable<T> GetBaseQuery()
        {
            List<string> listaIgnorada = ["CriadoPorUsuario", "AlteradoPorUsuario", "Empresa"];

            var query = _context.Set<T>().AsQueryable();

            // Buscar todas as propriedades virtuais da entidade
            var virtualProperties = typeof(T).GetProperties()
                .Where(p => p.GetGetMethod()?.IsVirtual == true
                    && !p.GetGetMethod()?.IsFinal == true
                    && p.PropertyType.IsClass
                    && p.PropertyType != typeof(string)
                    && !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType))
                .ToList();

            // Aplicar Include para cada propriedade virtual
            foreach (var prop in virtualProperties.Where(x => !listaIgnorada.Contains(x.Name)))
            {
                query = query.Include(prop.Name);
            }

            return query.OrderByDescending(x => x.Id);
        }

        protected virtual IQueryable<T> ApplyFilters(IQueryable<T> query, Dictionary<string, object> filters)
        {
            var stringProperties = typeof(T).GetProperties()
               .Where(p => p.PropertyType == typeof(string))
               .ToList();

            if (stringProperties.Count == 0)
            {
                return query;
            }

            Expression<Func<T, bool>> searchExpression = null;
            var parameter = Expression.Parameter(typeof(T), typeof(T).Name);

            foreach (var filter in filters)
            {
                foreach (var prop in stringProperties)
                {
                    var property = Expression.Property(parameter, prop.Name);
                    var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                    var searchValue = Expression.Constant(filters, typeof(string));
                    var nullCheck = Expression.NotEqual(property, Expression.Constant(null));
                    var contains = Expression.Call(property, containsMethod, searchValue);
                    var condition = Expression.AndAlso(nullCheck, contains);

                    searchExpression = searchExpression == null
                        ? Expression.Lambda<Func<T, bool>>(condition, parameter)
                        : Expression.Lambda<Func<T, bool>>(
                            Expression.OrElse(searchExpression.Body, condition), parameter);
                }
            }

            return searchExpression != null
                ? query.Where(searchExpression)
                : query;
        }

        protected virtual IQueryable<T> ApplySort(IQueryable<T> query, string orderBy, string orderDirection)
        {
            var property = typeof(T).GetProperty(orderBy);
            if (property == null)
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            var methodName = orderDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var resultExp = Expression.Call(typeof(Queryable), methodName,
                [typeof(T), property.PropertyType], query.Expression, orderByExp);

            return query.Provider.CreateQuery<T>(resultExp);
        }

        protected virtual List<SelectListItem> GetSelectOptions(string propertyName)
        {
            var autoOptions = StandardGridController<T>.GetAutoEnumOptions(propertyName);
            if (autoOptions.Count != 0)
            {
                return autoOptions;
            }

            // Se não for Enum, permite sobrescrita manual pelos controllers filhos
            return GetCustomSelectOptions(propertyName);
        }

        protected virtual List<SelectListItem> GetCustomSelectOptions(string propertyName)
        {
            return [];
        }

        protected virtual void ConfigureFormFields(List<FormFieldViewModel> fields, T entity, string action)
        {
        }

        protected virtual bool CanCreate(T? entity)
        {
            return true;
        }

        protected virtual bool CanEdit(T entity)
        {
            return true;
        }

        protected virtual bool CanDelete(T entity)
        {
            return true;
        }

        protected virtual void ProcessFileFieldsFromRequest(T entity)
        {
            var fileProperties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<FormFieldAttribute>() != null &&
                           (p.GetCustomAttribute<FormFieldAttribute>().Type == EnumFieldType.File ||
                            p.GetCustomAttribute<FormFieldAttribute>().Type == EnumFieldType.Image))
                .ToList();

            foreach (var property in fileProperties)
            {
                var propertyName = property.Name;

                // Obter valor do Request.Form (campo hidden)
                if (Request.Form.TryGetValue(propertyName, out var formValue))
                {
                    var value = formValue.ToString();

                    _logger?.LogInformation("Processando campo arquivo {Property}: '{Value}'", propertyName, value);

                    // Se tiver valor, atribuir à propriedade
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        property.SetValue(entity, value);
                    }
                    else
                    {
                        // Se valor vazio, manter null
                        property.SetValue(entity, null);
                    }
                }
            }
        }

        protected virtual Task BeforeCreate(T entity)
        {
            ProcessFileFieldsFromRequest(entity); // ADICIONE ESTA LINHA
            entity.DataCadastro = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        protected virtual Task BeforeUpdate(T entity)
        {
            ProcessFileFieldsFromRequest(entity); // ADICIONE ESTA LINHA
            entity.DataAlteracao = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        protected virtual Task BeforeDelete(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterCreate(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterUpdate(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterDelete(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual long GetCurrentEmpresaId()
        {
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            return long.TryParse(empresaIdClaim, out var empresaId)
                ? empresaId
                : 1;
        }

        #endregion

        #region Actions/EndPoints para Formulários Dinâmicos

        /// <summary>
        /// GET: Index - Exibir tela inicial com grid
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Index(
            string? search = null,
            string? orderBy = null,
            string? orderDirection = "asc",
            int pageSize = 50,
            int page = 1)
        {
            var gridConfig = ConfigureGrid(ControllerContext.ActionDescriptor.ControllerName);
            ConfigureCustomGrid(gridConfig);
            var query = GetBaseQuery();

            // Aplicar filtros
            var filters = ExtractFiltersFromRequest();
            if (!string.IsNullOrEmpty(search))
            {
                filters["search"] = search;
            }

            query = ApplyFilters(query, filters);

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplySort(query, orderBy, orderDirection);
            }

            // Obter total de registros
            var totalRecords = await query.CountAsync();

            // Aplicar paginação
            List<T> items;
            if (pageSize == -1)
            {
                items = await query.ToListAsync();
            }
            else
            {
                items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            // Configurar ViewModel
            gridConfig.Items = [.. items.Cast<object>()];
            gridConfig.TotalRecords = totalRecords;
            gridConfig.CurrentPage = page;
            gridConfig.PageSize = pageSize;
            gridConfig.Search = search;
            gridConfig.OrderBy = orderBy;
            gridConfig.OrderDirection = orderDirection;

            // Atualizar valores dos filtros
            UpdateFilterValues(gridConfig.Filters, filters);

            return View("_StandardGridContent", gridConfig);
        }

        /// <summary>
        /// Endpoint AJAX para atualização dinâmica da grid
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> GetDataAjax(
            string? search = null,
            string? orderBy = null,
            string? orderDirection = "asc",
            int pageSize = 50,
            int page = 1)
        {
            var gridConfig = ConfigureGrid(ControllerContext.ActionDescriptor.ControllerName);
            var query = GetBaseQuery();

            // Aplicar filtros
            var filters = ExtractFiltersFromRequest();
            if (!string.IsNullOrEmpty(search))
            {
                filters["search"] = search;
            }

            query = ApplyFilters(query, filters);

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = ApplySort(query, orderBy, orderDirection);
            }

            // Obter total de registros
            var totalRecords = await query.CountAsync();

            // Aplicar paginação
            List<T> items;
            if (pageSize == -1)
            {
                items = await query.ToListAsync();
            }
            else
            {
                items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            // Configurar ViewModel
            gridConfig.Items = [.. items.Cast<object>()];
            gridConfig.TotalRecords = totalRecords;
            gridConfig.CurrentPage = page;
            gridConfig.PageSize = pageSize;
            gridConfig.Search = search;
            gridConfig.OrderBy = orderBy;
            gridConfig.OrderDirection = orderDirection;

            // Atualizar valores dos filtros
            UpdateFilterValues(gridConfig.Filters, filters);

            return PartialView("_StandardGridContent", gridConfig);
        }

        /// <summary>
        /// GET: Create - Exibir formulário de criação
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var entity = new T();
            await BeforeCreate(entity);

            // DETECTAR SE É MODAL
            var isModal = Request.Query.ContainsKey("modal") && Request.Query["modal"] == "true";
            _logger?.LogInformation("Create chamado - IsModal: {IsModal}", isModal);
            if (isModal)
            {
                // Retornar apenas o formulário sem layout para o modal
                var formViewModel = await BuildFormViewModelAsync(entity, "Create");

                _logger?.LogInformation("Retornando _ModalForm para modal");

                // IMPORTANTE: Usar PartialView para não carregar layout
                return PartialView("_ModalForm", formViewModel);
            }

            // Comportamento normal com tabs
            if (ShouldUseTabbedForm())
            {
                var viewModel = await BuildTabbedFormViewModelAsync(entity, "Create");
                return View("_TabbedForm", viewModel);
            }

            var standardViewModel = await BuildFormViewModelAsync(entity, "Create");
            return View("_StandardForm", standardViewModel);
        }

        /// <summary>
        /// POST: Create - Processar criação da entidade
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(T entity)
        {
            if (!CanCreate(entity))
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        sucesso = false,
                        mensagem = "Você não tem permissão para cadastrar um novo registro.",
                        script = "showError('Você não tem permissão para cadastrar um novo registro.')"
                    });
                }

                TempData["NotificationScript"] = "showError('Você não tem permissão para cadastrar um novo registro.')";
                return Forbid();
            }

            // Requisições AJAX (incluindo modais) usam HandleModalCreate
            if (IsAjaxRequest())
            {
                return await HandleModalCreate(this, entity);
            }

            // Fluxo padrão para formulário normal (não-AJAX)
            if (ModelState.IsValid)
            {
                try
                {
                    await BeforeCreate(entity);
                    _context.Set<T>().Add(entity);
                    await _context.SaveChangesAsync();
                    await AfterCreate(entity);

                    TempData["NotificationScript"] = "showSuccess('Registro cadastrado com sucesso!')";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar registro: {ex.Message}");
                }
            }

            var viewModel = await BuildFormViewModelAsync(entity, "Create");
            AddModelStateToViewModel(viewModel);
            return View("_StandardForm", viewModel);
        }


        /// <summary>
        /// GET: Edit - Exibir formulário de edição
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = GetBaseQuery();
            var entityType = typeof(T);

            var navigationProperties = entityType.GetProperties()
                .Where(p => p.PropertyType.IsClass &&
                            p.PropertyType != typeof(string) &&
                            !p.PropertyType.IsArray &&
                            p.GetCustomAttribute<FormFieldAttribute>()?.Type == EnumFieldType.Reference);

            foreach (var navProp in navigationProperties)
            {
                var propertyName = navProp.Name.StartsWith("Id") ? navProp.Name.Substring(2) : navProp.Name;
                var property = entityType.GetProperty(propertyName);

                if (property != null)
                {
                    query = query.Include(propertyName);
                }
            }

            var entity = await query.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            await BeforeUpdate(entity);

            var isModal = Request.Query.ContainsKey("modal") && Request.Query["modal"] == "true";

            _logger?.LogInformation("Edit chamado - Id: {Id}, IsModal: {IsModal}", id, isModal);

            if (isModal)
            {
                var formViewModel = await BuildFormViewModelAsync(entity, "Edit");
                _logger?.LogInformation("Retornando _ModalForm para modal");
                return PartialView("_ModalForm", formViewModel);
            }

            if (ShouldUseTabbedForm())
            {
                var viewModel = await BuildTabbedFormViewModelAsync(entity, "Edit");
                return View("_TabbedForm", viewModel);
            }

            var standardViewModel = await BuildFormViewModelAsync(entity, "Edit");
            return View("_StandardForm", standardViewModel);
        }

        /// <summary>
        /// POST: Edit - Processar edição da entidade
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(long id, T entity)
        {
            var allProperties = typeof(T).GetProperties();
            foreach (var prop in allProperties)
            {
                var value = prop.GetValue(entity);
            }

            // Verificar especificamente campos de arquivo
            var fileProperties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<FormFieldAttribute>()?.Type == EnumFieldType.File ||
                            p.GetCustomAttribute<FormFieldAttribute>()?.Type == EnumFieldType.Image);

            foreach (var prop in fileProperties)
            {
                var value = prop.GetValue(entity);
                Console.WriteLine($"{prop.Name}: '{value}' (IsNull: {value == null})");
            }

            if (id != entity.Id)
            {
                return Request.IsAjaxRequest()
                    ? Json(new { success = false, message = "ID inconsistente." })
                    : NotFound();
            }

            var existingEntity = await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id);
            if (existingEntity == null)
            {
                return Request.IsAjaxRequest()
                    ? Json(new { success = false, message = "Registro não encontrado." })
                    : NotFound();
            }

            if (!CanEdit(existingEntity))
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { sucesso = false, mensagem = "Você não tem permissão para editar este registro.", script = "showError('Você não tem permissão para editar este registro.')" });
                }

                TempData["NotificationScript"] = "showError('Você não tem permissão para editar este registro.')";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preservar valores
                    entity.IdEmpresa = existingEntity.IdEmpresa;
                    entity.DataCadastro = existingEntity.DataCadastro;
                    entity.CriadoPorUsuarioId = existingEntity.CriadoPorUsuarioId;

                    // CRITICAL: Preservar campos de arquivo que não foram alterados
                    foreach (var prop in fileProperties)
                    {
                        var newValue = prop.GetValue(entity);
                        var oldValue = prop.GetValue(existingEntity);

                        // Se o novo valor está vazio mas havia um arquivo antes, manter o antigo
                        if (string.IsNullOrEmpty(newValue?.ToString()) && !string.IsNullOrEmpty(oldValue?.ToString()))
                        {
                            prop.SetValue(entity, oldValue);
                        }
                    }

                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);

                    await BeforeUpdate(existingEntity);
                    await _context.SaveChangesAsync();
                    await AfterUpdate(existingEntity);

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new
                        {
                            sucesso = true,
                            mensagem = "Registro atualizado com sucesso!",
                            script = "showSuccess('Registro atualizado com sucesso!').then(() => window.location.reload())"
                        });
                    }

                    TempData["NotificationScript"] = "showSuccess('Registro atualizado com sucesso!')";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new
                        {
                            sucesso = false,
                            mensagem = $"Erro ao atualizar: {ex.Message}",
                            script = $"showError('Erro ao atualizar: {ex.Message}')"
                        });
                    }

                    TempData["NotificationScript"] = $"showError('Erro ao atualizar: {ex.Message}')";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Any())
                    {
                        Console.WriteLine($"Erro validação {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }

            if (Request.IsAjaxRequest())
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new
                {
                    success = false,
                    message = "Erro de validação.",
                    errors
                });
            }

            var viewModel = await BuildFormViewModelAsync(entity, "Edit");
            AddModelStateToViewModel(viewModel);

            return View("_StandardForm", viewModel);
        }

        /// <summary>
        /// GET: Details - Exibir detalhes da entidade
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Details(long id)
        {
            var query = GetBaseQuery();

            var entityType = typeof(T);
            var navigationProperties = entityType.GetProperties()
                .Where(p => p.PropertyType.IsClass &&
                            p.PropertyType != typeof(string) &&
                            !p.PropertyType.IsArray &&
                            p.GetCustomAttribute<FormFieldAttribute>()?.Type == EnumFieldType.Reference);

            foreach (var navProp in navigationProperties)
            {
                var propertyName = navProp.Name.StartsWith("Id") ? navProp.Name.Substring(2) : navProp.Name;
                var property = entityType.GetProperty(propertyName);

                if (property != null)
                {
                    query = query.Include(propertyName);
                }
            }

            var entity = await query.FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            await this.AddAuditHistoryToViewBag(_context, entity);

            var viewModel = await BuildFormViewModelAsync(entity, "Details"); // USAR VERSÃO ASYNC

            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>();
            if (formTabs?.EnableTabs == true)
            {
                viewModel = await BuildTabbedFormViewModelAsync(entity, "Details");
                return View("_TabbedForm", viewModel);
            }
            else
            {
                return View("_StandardForm", viewModel);
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> GerarRelatorio(long id)
        {
            var entity = await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            // Obter template padrão e gerar HTML
            var template = _reportService.GetDefaultTemplate<T>();
            var html = _reportService.GenerateReportHtml(entity, template);

            return Content(html, "text/html");
        }

        /// <summary>
        /// POST: Delete - Excluir entidade
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var entity = await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            if (!CanDelete(entity))
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        sucesso = false,
                        mensagem = "Você não tem permissão para excluir este registro.",
                        script = "showError('RVocê não tem permissão para excluir este registro.!').then(() => window.location.reload())"
                    });
                }

                TempData["NotificationScript"] = "showError('Você não tem permissão para excluir este registro.')";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await BeforeDelete(entity);
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                await AfterDelete(entity);

                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        sucesso = true,
                        mensagem = "Registro excluído com sucesso!",
                        script = "showSuccess('Registro excluído com sucesso!').then(() => window.location.reload())"
                    });
                }

                TempData["NotificationScript"] = "showSuccess('Registro excluído com sucesso!')";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new
                    {
                        sucesso = false,
                        mensagem = $"Erro ao excluir registro: {ex.Message}",
                        script = $"showError('Erro ao excluir registro: {ex.Message}')"
                    });
                }

                TempData["NotificationScript"] = $"showError('Erro ao excluir registro: {ex.Message}')";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> Export()
        {
            try
            {
                // Obter configuração de colunas automaticamente
                var columns = GridColumnBuilder.BuildColumns<T>()
                    .Where(c => c.Type != EnumGridColumnType.Actions)
                    .ToList();

                // Aplicar query base e filtros
                var filters = ExtractFiltersFromRequest();
                var query = GetBaseQuery();
                query = ApplyFilters(query, filters);

                // Obter dados
                var data = await query.ToListAsync();

                // Gerar CSV
                var csv = new System.Text.StringBuilder();

                // Header
                var headers = columns.Select(c => c.DisplayName);
                csv.AppendLine(string.Join(",", headers));

                // Dados
                foreach (var item in data)
                {
                    var values = columns.Select(column => FormatValueForExport(item, column));
                    var line = string.Join(",", values.Select(v => EscapeCsvValue(v)));
                    csv.AppendLine(line);
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var entityName = typeof(T).Name.ToLower();
                var fileName = $"{entityName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["NotificationScript"] = $"showError('Erro ao exportar dados: {ex.Message}')";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> UploadFile(string propertyName, IFormFile file, string? customBucket = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { sucesso = false, mensagem = "Nenhum arquivo selecionado", script = "showError('Nenhum arquivo selecionado')" });
                }

                var property = typeof(T).GetProperties()
                    .FirstOrDefault(p => p.Name == propertyName &&
                        (p.GetCustomAttribute<FormFieldAttribute>()?.Type == EnumFieldType.File ||
                         p.GetCustomAttribute<FormFieldAttribute>()?.Type == EnumFieldType.Image));

                if (property == null)
                {
                    return Json(new { sucesso = false, mensagem = "Campo não encontrado", script = "showError('Campo não encontrado')" });
                }

                var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();

                // Validar extensão
                if (!string.IsNullOrEmpty(formFieldAttr.AllowedExtensions))
                {
                    var allowedExts = formFieldAttr.AllowedExtensions
                        .Split(',')
                        .Select(e => e.Trim().ToLower())
                        .ToList();

                    var fileExt = Path.GetExtension(file.FileName).ToLower().TrimStart('.');

                    if (!allowedExts.Contains(fileExt))
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Extensão não permitida. Use: {formFieldAttr.AllowedExtensions}"
                        });
                    }
                }

                // Validar tamanho
                var maxSize = formFieldAttr.MaxSizeMB * 1024 * 1024;
                if (file.Length > maxSize)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Arquivo muito grande. Máximo: {formFieldAttr.MaxSizeMB}MB"
                    });
                }

                // Upload para MinIO
                var entityName = typeof(T).Name;
                var idEmpresa = GetCurrentEmpresaId();
                var filePath = await _fileStorageService.UploadFileAsync(file, entityName, propertyName, idEmpresa, customBucket);

                // Gerar URL de download
                var fileUrl = await _fileStorageService.GetDownloadUrlAsync(filePath, entityName, idEmpresa, customBucket);
                return Json(new
                {
                    success = true,
                    fileName = Path.GetFileName(file.FileName),
                    filePath,
                    fileUrl,
                    message = "Arquivo enviado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao fazer upload do arquivo");
                return Json(new { success = false, message = $"Erro ao enviar arquivo: {ex.Message}" });
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> DownloadFile(string propertyName, string filePath, string? customBucket = null)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger?.LogWarning("Tentativa de download com filePath vazio. PropertyName: {PropertyName}", propertyName);
                    return NotFound("Arquivo não encontrado");
                }

                var entityName = typeof(T).Name;
                var idEmpresa = GetCurrentEmpresaId();

                var stream = await _fileStorageService.DownloadFileAsync(
                    filePath,
                    entityName,
                    idEmpresa,
                    customBucket);

                var contentType = GetContentType(filePath);
                var downloadName = Path.GetFileName(filePath);

                return File(stream, contentType, downloadName);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao fazer download do arquivo. PropertyName: {PropertyName}, FilePath: {FilePath}", propertyName, filePath);
                return NotFound("Arquivo não encontrado");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteFile([FromBody] DeleteFileRequestModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FilePath))
                {
                    _logger?.LogWarning("Tentativa de exclusão com filePath vazio. PropertyName: {PropertyName}", request.PropertyName);
                    return Json(new { sucesso = false, mensagem = "Caminho do arquivo não informado", script = "showError('Caminho do arquivo não informado')" });
                }

                var entityName = typeof(T).Name;
                var idEmpresa = GetCurrentEmpresaId();

                var deleted = await _fileStorageService.DeleteFileAsync(
                    request.FilePath,
                    entityName,
                    idEmpresa,
                    request.CustomBucket);

                if (deleted)
                {
                    return Json(new { sucesso = true, mensagem = "Arquivo excluído com sucesso!", script = "showSuccess('Arquivo excluído com sucesso!')" });
                }

                return Json(new { sucesso = false, mensagem = "Erro ao excluir arquivo", script = "showError('Erro ao excluir arquivo')" });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao excluir arquivo. PropertyName: {PropertyName}, FilePath: {FilePath}",
                    request.PropertyName, request.FilePath);
                return Json(new { success = false, message = $"Erro ao excluir arquivo: {ex.Message}" });
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetFileUrl(string propertyName, string fileName, string? customBucket = null)
        {
            try
            {
                var entityName = typeof(T).Name;
                var idEmpresa = GetCurrentEmpresaId();
                var url = await _fileStorageService.GetDownloadUrlAsync(fileName, entityName, idEmpresa, customBucket);
                return Json(new { success = true, url });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao obter URL do arquivo");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        public void AddModelStateToViewModel(StandardFormViewModel viewModel)
        {
            viewModel.ModelState.Clear();
            foreach (var error in ModelState)
            {
                if (error.Value.Errors.Any())
                {
                    viewModel.ModelState[error.Key] = string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage));
                }
            }
        }

        public bool IsAjaxRequest()
        {
            return Request.Headers.ContainsKey("X-Requested-With") || Request.Query.ContainsKey("ajax");
        }

        #region Métodos para Sistema de Abas

        /// <summary>
        /// Configurar abas da entidade
        /// </summary>
        protected virtual List<FormTabViewModel> ConfigureTabs(T entity)
        {
            var tabs = new List<FormTabViewModel>
            {
                // Aba principal sempre existe
                new() {
                    TabId = "principal",
                    TabName = "Dados Principais",
                    TabIcon = "fas fa-info-circle",
                    Order = 0,
                    LazyLoad = false,
                    IsActive = true
                }
            };

            // Buscar abas configuradas via atributos
            var tabAttributes = typeof(T).GetCustomAttributes<FormTabAttribute>()
                .OrderBy(t => t.Order)
                .ToList();

            foreach (var tabAttr in tabAttributes)
            {
                var tab = new FormTabViewModel
                {
                    TabId = tabAttr.TabId,
                    TabName = tabAttr.TabName,
                    TabIcon = tabAttr.TabIcon,
                    Order = tabAttr.Order,
                    Controller = tabAttr.Controller,
                    Action = tabAttr.Action,
                    LazyLoad = tabAttr.LazyLoad,
                    HasAccess = HasTabAccess(tabAttr.RequiredRoles)
                };

                // Adicionar parâmetros para a aba
                if (entity != null)
                {
                    tab.Parameters.Add("id", entity.Id);
                    tab.Parameters.Add("entityType", typeof(T).Name);
                }

                tabs.Add(tab);
            }

            return tabs;
        }

        /// <summary>
        /// Verificar se o usuário tem acesso à aba
        /// </summary>
        protected virtual bool HasTabAccess(string[] requiredRoles)
        {
            return requiredRoles == null || requiredRoles.Length == 0 ? true : requiredRoles.Any(role => User.IsInRole(role));
        }

        protected virtual async Task<StandardFormViewModel> BuildFormViewModelAsync(T entity, string action)
        {
            var formConfig = typeof(T).GetCustomAttribute<FormConfigAttribute>() ?? new FormConfigAttribute();
            var properties = GetFormProperties();

            var viewModel = new StandardFormViewModel
            {
                Title = formConfig.Title ?? StandardGridController<T>.GetDefaultTitle(action),
                Subtitle = formConfig.Subtitle ?? GetDefaultSubtitle(action),
                Icon = formConfig.Icon ?? "fas fa-edit",
                BackAction = formConfig.BackAction ?? "Index",
                BackText = formConfig.BackText ?? "Voltar à Lista",
                ActionName = action,
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                Model = entity,
                EnableAjaxSubmit = formConfig.EnableAjaxSubmit,
                IsEditMode = action == "Edit",
                IsDetailsMode = action == "Details"
            };

            // CRIAR TASKS PARA PROCESSAR PROPRIEDADES DE FORMA ASSÍNCRONA
            var fieldTasks = properties.Select(p => CreateFieldFromPropertyAsync(p, entity, action));
            var fields = await Task.WhenAll(fieldTasks);

            // Agrupar campos por seção
            var fieldsBySection = fields
                .Where(f => f != null)
                .GroupBy(f => f!.Section ?? "Não Informado")
                .ToList();

            foreach (var sectionGroup in fieldsBySection)
            {
                var section = new FormSectionViewModel
                {
                    Name = sectionGroup.Key,
                    Icon = GetSectionIcon(sectionGroup.Key),
                    Fields = sectionGroup.OrderBy(f => f!.Order).ToList()!
                };

                section.GridColumns = section.Fields.Count != 0 ? section.Fields.Max(f => f.GridColumns) : 1;
                viewModel.Sections.Add(section);
            }

            var allFields = viewModel.Sections.SelectMany(s => s.Fields).ToList();
            ConfigureFormFields(allFields, entity, action);

            return viewModel;
        }

        #endregion

        #region Métodos Privados

        private static bool ShouldUseTabbedForm()
        {
            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>();
            return formTabs?.EnableTabs ?? false;
        }

        private Dictionary<string, object> ExtractFiltersFromRequest()
        {
            var filters = new Dictionary<string, object>();

            foreach (var key in Request.Query.Keys)
            {
                var value = Request.Query[key].ToString();

                if (!string.IsNullOrEmpty(value) &&
                    !new[] { "page", "pageSize", "orderBy", "orderDirection" }.Contains(key))
                {
                    // Tratamento especial para filtros de data range
                    if (key.EndsWith("_inicio") || key.EndsWith("_fim"))
                    {
                        filters[key] = value;
                    }
                    else
                    {
                        filters[key] = value;
                    }
                }
            }

            return filters;
        }

        private void UpdateFilterValues(List<GridFilter> filters, Dictionary<string, object> values)
        {
            foreach (var filter in filters)
            {
                if (values.TryGetValue(filter.Name, out var value))
                {
                    filter.Value = value;
                }

                // Para filtros de data range, verificar campos específicos
                if (filter.Type == EnumGridFilterType.DateRange)
                {
                    var inicioKey = $"{filter.Name}_inicio";
                    var fimKey = $"{filter.Name}_fim";

                    if (values.TryGetValue(inicioKey, out var valueInico))
                    {
                        ViewBag.GetType().GetProperty(inicioKey)?.SetValue(ViewBag, valueInico);
                    }

                    if (values.TryGetValue(fimKey, out var valueFim))
                    {
                        ViewBag.GetType().GetProperty(fimKey)?.SetValue(ViewBag, valueFim);
                    }
                }
            }
        }

        private async Task<TabbedFormViewModel> BuildTabbedFormViewModelAsync(T entity, string action)
        {
            var formConfig = typeof(T).GetCustomAttribute<FormConfigAttribute>() ?? new FormConfigAttribute();
            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>() ?? new FormTabsAttribute();

            var viewModel = new TabbedFormViewModel
            {
                Title = formConfig.Title ?? StandardGridController<T>.GetDefaultTitle(action),
                Subtitle = formConfig.Subtitle ?? GetDefaultSubtitle(action),
                Icon = formConfig.Icon ?? "fas fa-edit",
                BackAction = formConfig.BackAction ?? "Index",
                BackText = formConfig.BackText ?? "Voltar à Lista",
                ActionName = action,
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                Model = entity,
                EnableAjaxSubmit = formConfig.EnableAjaxSubmit,
                IsEditMode = action == "Edit",
                IsDetailsMode = action == "Details",
                EnableTabs = formTabs.EnableTabs,
                EntityId = entity.Id,
                ActiveTab = formTabs.DefaultTab ?? "principal",
                // Configurar abas
                Tabs = ConfigureTabs(entity)
            };

            // Construir formulário principal para a primeira aba
            var mainFormViewModel = await BuildFormViewModelAsync(entity, action);
            viewModel.Sections = mainFormViewModel.Sections;
            viewModel.ModelState = mainFormViewModel.ModelState;

            return viewModel;
        }

        private bool ViewExists(string viewName)
        {
            var viewEngine = HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine))
                as Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine;

            var viewResult = viewEngine?.FindView(ControllerContext, viewName, false);
            return viewResult?.Success ?? false;
        }

        private static StandardGridViewModel ConfigureGrid(string controllerName)
        {
            var standardGridViewModel = new StandardGridViewModel
            {
                Title = controllerName,
                SubTitle = $"Gerencie todos os {controllerName}",
                EntityName = typeof(T).Name,
                ControllerName = controllerName,
                Columns = GridColumnBuilder.BuildColumns<T>(),
                HeaderActions = ObterHeaderActionsPadrao(controllerName),
                RowActions = ObterRowActionsPadrao(controllerName)
            };

            return standardGridViewModel;
        }

        private static bool ShouldAutoGenerateField(PropertyInfo property)
        {
            // Não incluir propriedades de navegação
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                return false;
            }

            // Não incluir coleções
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) &&
                property.PropertyType != typeof(string))
            {
                return false;
            }

            // Não incluir propriedades comuns que são gerenciadas automaticamente
            var ignoredNames = new[] { "Id", "DataCadastro", "DataAlteracao", "UsuarioCadastro", "UsuarioAlteracao", "CriadoPorUsuarioId", "AlteradoPorUsuarioId" };
            return !ignoredNames.Contains(property.Name);
        }

        private static List<GridAction> ObterHeaderActionsPadrao(string controllerNome)
        {
            return
                [
                    new()
                    {
                        Name = "Export",
                        DisplayName = "Exportar",
                        Icon = "fas fa-download",
                        CssClass = "btn-modern btn-outline-modern",
                        Url = "/" + controllerNome + "/Export"
                    },
                    new()
                    {
                        Name = "Create",
                        DisplayName = "Novo ",
                        Icon = "fas fa-plus",
                        CssClass = "btn-new",
                        Url = "/" + controllerNome + "/Create"
                    }
                ];
        }

        private static List<GridAction> ObterRowActionsPadrao(string controllerNome)
        {
            return
                [
                    new()
                    {
                        Name = "Details",
                        DisplayName = "Visualizar",
                        Icon = "fas fa-eye",
                        Url = "/" + controllerNome + "/Details/{id}"
                    },
                    new()
                    {
                        Name = "Edit",
                        DisplayName = "Editar",
                        Icon = "fas fa-edit",
                        Url = "/" + controllerNome + "/Edit/{id}"
                    },
                    new()
                    {
                        Name = "QuickReport",
                        DisplayName = "PDF",
                        Icon = "fas fa-file-pdf",
                        CssClass = "btn btn-sm btn-outline-success",
                        Url = "/" + controllerNome + "/GerarRelatorio/{id}",
                    },
                    new()
                    {
                        Name = "Delete",
                        DisplayName = "Excluir",
                        Icon = "fas fa-trash",
                        Url = "/" + controllerNome + "/Delete/{id}"
                    },
                ];
        }

        private static List<PropertyInfo> GetFormProperties()
        {
            return [.. typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<FormFieldAttribute>() != null)
                .OrderBy(p => p.GetCustomAttribute<FormFieldAttribute>()?.Order ?? 0)];
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        private static bool TryConvertToNumeric<TProperty>(string value, out TProperty result)
        {
            result = default;

            try
            {
                result = (TProperty)Convert.ChangeType(value, typeof(TProperty));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(long) || type == typeof(decimal) ||
                   type == typeof(double) || type == typeof(float) || type == typeof(short);
        }

        private static bool IsIgnoredProperty(PropertyInfo property)
        {
            return property.GetCustomAttribute<FormFieldAttribute>() == null &&
                   !StandardGridController<T>.ShouldAutoGenerateField(property);
        }

        public static string GetDisplayName(PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>();
            if (display?.Name != null)
            {
                return display.Name;
            }

            var displayName = property.GetCustomAttribute<DisplayNameAttribute>();
            if (displayName?.DisplayName != null)
            {
                return displayName.DisplayName;
            }

            // Adicionar espaços antes de letras maiúsculas
            return System.Text.RegularExpressions.Regex.Replace(property.Name, "([a-z])([A-Z])", "$1 $2");
        }

        private static string GetDefaultIcon(PropertyInfo property)
        {
            var propertyName = property.Name.ToLower();

            return propertyName switch
            {
                var name when name.Contains("nome") => "fas fa-signature",
                var name when name.Contains("email") => "fas fa-envelope",
                var name when name.Contains("telefone") || name.Contains("celular") => "fas fa-phone",
                var name when name.Contains("endereco") => "fas fa-road",
                var name when name.Contains("data") => "fas fa-calendar",
                var name when name.Contains("valor") || name.Contains("preco") => "fas fa-dollar-sign",
                "cpf" => "fas fa-fingerprint",
                "cnpj" => "fas fa-building",
                _ => "fas fa-edit"
            };
        }

        private static string GetDefaultPlaceholder(PropertyInfo property)
        {
            var propertyName = property.Name.ToLower();
            var fieldType = DetermineFieldType(property);

            return fieldType switch
            {
                EnumFieldType.Email => "exemplo@email.com",
                EnumFieldType.Telefone => "(00) 00000-0000",
                EnumFieldType.Cpf => "000.000.000-00",
                EnumFieldType.Cnpj => "00.000.000/0000-00",
                EnumFieldType.Cep => "00000-000",
                EnumFieldType.Currency => "R$ 0,00",
                EnumFieldType.Date => "dd/mm/aaaa",
                EnumFieldType.TextArea => $"Digite as {GetDisplayName(property).ToLower()}...",
                _ => $"Digite {GetDisplayName(property).ToLower()}"
            };
        }

        private static string GetSectionIcon(string sectionName)
        {
            return sectionName.ToLower() switch
            {
                "dados básicos" => "fas fa-info-circle",
                "contato" => "fas fa-phone",
                "endereço" => "fas fa-map-marker-alt",
                "financeiro" => "fas fa-dollar-sign",
                "observações" => "fas fa-sticky-note",
                _ => "fas fa-edit"
            };
        }

        private static string GetDefaultTitle(string action)
        {
            var entityName = typeof(T).Name;
            return action switch
            {
                "Create" => $"Novo {entityName}",
                "Edit" => $"Editar {entityName}",
                "Details" => $"Detalhes {entityName}",
                _ => entityName
            };
        }

        private static string GetDefaultSubtitle(string action)
        {
            var entityName = typeof(T).Name.ToLower();
            return action switch
            {
                "Create" => $"Cadastre um novo {entityName} no sistema",
                "Edit" => $"Altere as informações do {entityName}",
                "Details" => $"Visualize as informações do {entityName}",
                _ => $"Gerencie as informações do {entityName}"
            };
        }

        private static string FormatValueForExport(T item, GridColumn column)
        {
            try
            {
                // Para campos compostos, usar o CustomRender se existir
                if (column.Type == EnumGridColumnType.Custom && column.CustomRender != null)
                {
                    var rendered = column.CustomRender(item);
                    // Remover tags HTML
                    return System.Text.RegularExpressions.Regex.Replace(rendered, "<.*?>", string.Empty).Trim();
                }

                // Obter valor da propriedade
                var property = typeof(T).GetProperty(column.Name);
                if (property == null)
                {
                    return "";
                }

                var value = property.GetValue(item);
                if (value == null)
                {
                    return "";
                }

                // Formatação por tipo
                return column.Type switch
                {
                    EnumGridColumnType.Date => value is DateTime date ? date.ToString("dd/MM/yyyy") : value.ToString() ?? "",
                    EnumGridColumnType.Number => FormatNumber(value),
                    EnumGridColumnType.Enumerador => FormatEnum(value),
                    _ => value.ToString() ?? ""
                };
            }
            catch
            {
                return "";
            }
        }

        private static string FormatNumber(object value)
        {
            if (value == null)
            {
                return "";
            }

            return value switch
            {
                decimal d => d.ToString("F2"),
                double db => db.ToString("F2"),
                float f => f.ToString("F2"),
                _ => value.ToString() ?? ""
            };
        }

        private static string FormatEnum(object value)
        {
            if (value == null)
            {
                return "";
            }

            var type = value.GetType();
            if (type.IsEnum)
            {
                return value.GetDescription();
            }

            // Para Nullable<Enum>
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType?.IsEnum == true)
            {
                return value.GetDescription();
            }

            if (value is bool boolValue)
            {
                return boolValue ? "Sim" : "Não";
            }

            return value.ToString() ?? "";
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            // Se contém vírgula, aspas ou quebra de linha, envolver em aspas
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                // Duplicar aspas internas
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }

        #endregion

        #region Automated Enum Detection and Population

        private static EnumFieldType DetermineFieldType(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (type.IsEnum)
            {
                return EnumFieldType.Select;
            }

            return type switch
            {
                Type t when t == typeof(string) => EnumFieldType.Text,
                Type t when t == typeof(int) || t == typeof(long) => EnumFieldType.Number,
                Type t when t == typeof(decimal) || t == typeof(double) || t == typeof(float) => EnumFieldType.Currency,
                Type t when t == typeof(DateTime) => EnumFieldType.Date,
                Type t when t == typeof(bool) => EnumFieldType.Checkbox,
                _ => EnumFieldType.Text
            };
        }

        private static List<SelectListItem> GetAutoEnumOptions(string propertyName)
        {
            try
            {
                // Obtém a propriedade da entidade
                var property = typeof(T).GetProperty(propertyName);
                if (property == null)
                {
                    return [];
                }

                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (!propertyType.IsEnum)
                {
                    return [];
                }

                // Usa reflexão para chamar EnumExtension.GetSelectListItems<TEnum>(true)
                var enumExtensionMethod = typeof(EnumExtension).GetMethod("GetSelectListItems");
                var genericMethod = enumExtensionMethod.MakeGenericMethod(propertyType);

                // Chama o método com obterIcone = true (você pode parametrizar isso se quiser)
                var result = genericMethod.Invoke(null, [true]) as List<SelectListItem>;

                return result ?? [];
            }
            catch (Exception ex)
            {
                // Log do erro mas não quebra o sistema
                Console.WriteLine($"Erro ao obter opções automáticas para Enum {propertyName}: {ex.Message}");
                return [];
            }
        }

        private async Task<FormFieldViewModel?> CreateFieldFromPropertyAsync(PropertyInfo property, T entity, string action)
        {
            var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();

            if (formFieldAttr != null)
            {
                var conditionalDisplayAttr = property.GetCustomAttribute<ConditionalDisplayAttribute>();
                var conditionalRequiredAttr = property.GetCustomAttribute<ConditionalRequiredAttribute>();

                var displayRule = "";
                if (conditionalDisplayAttr != null)
                {
                    displayRule = conditionalDisplayAttr.Rule;
                }

                var shouldDisplay = true;
                if (!string.IsNullOrEmpty(displayRule))
                {
                    shouldDisplay = ConditionalExpressionEvaluator.Evaluate(displayRule, entity, typeof(T));
                }

                var requiredRule = conditionalRequiredAttr?.Rule ?? "";
                var isConditionallyRequired = false;
                var requiredMessage = conditionalRequiredAttr?.ErrorMessage ?? "";

                if (!string.IsNullOrEmpty(requiredRule))
                {
                    isConditionallyRequired = ConditionalExpressionEvaluator.Evaluate(requiredRule, entity, typeof(T));
                }

                var isRequired = formFieldAttr.Required || isConditionallyRequired;

                string? referenceFilters = null;
                if (formFieldAttr.Type == EnumFieldType.Reference && ReferenceFilterHelper.HasFilters(property))
                {
                    var filterConfig = ReferenceFilterHelper.GetFilterConfig(property);
                    referenceFilters = ReferenceFilterHelper.SerializeFilterConfig(filterConfig);
                }

                var rawValue = property.GetValue(entity);

                // IMPORTANTE: Converter sempre para string formatada
                string? formattedValue = null;

                try
                {
                    if (rawValue != null)
                    {
                        formattedValue = FormatFieldValueToString(rawValue, formFieldAttr.Type, property);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Erro ao formatar campo {PropertyName} do tipo {Type}", property.Name, formFieldAttr.Type);
                    formattedValue = rawValue?.ToString();
                }

                string? displayText = null;
                if (formFieldAttr.Type == EnumFieldType.Reference && rawValue != null)
                {
                    displayText = GetReferenceDisplayText(entity, property, formFieldAttr.Reference);
                }

                string? fileUrl = null;
                string? fileName = null;
                string? filePath = null;

                if ((formFieldAttr.Type == EnumFieldType.File || formFieldAttr.Type == EnumFieldType.Image) && rawValue != null)
                {
                    var fileValue = rawValue.ToString();
                    if (!string.IsNullOrEmpty(fileValue))
                    {
                        filePath = fileValue;
                        fileName = Path.GetFileName(fileValue);
                        var entityName = typeof(T).Name;
                        var idEmpresa = GetCurrentEmpresaId();

                        try
                        {
                            fileUrl = await _fileStorageService.GetDownloadUrlAsync(fileValue, entityName, idEmpresa);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, "Erro ao gerar URL do arquivo: {FileName}", fileValue);
                        }
                    }
                }

                return new FormFieldViewModel
                {
                    PropertyName = property.Name,
                    DisplayName = formFieldAttr.Name ?? GetDisplayName(property),
                    Icon = formFieldAttr.Icon ?? GetDefaultIcon(property),
                    Placeholder = formFieldAttr.Placeholder ?? GetDefaultPlaceholder(property),
                    Type = formFieldAttr.Type,
                    Required = isRequired,
                    ReadOnly = action == "Details" || formFieldAttr.ReadOnly,
                    Value = formattedValue,
                    DisplayText = displayText,
                    Reference = formFieldAttr.Reference ?? null,
                    ValidationRegex = formFieldAttr.ValidationRegex ?? "",
                    ValidationMessage = formFieldAttr.ValidationMessage ?? "",
                    GridColumns = formFieldAttr.GridColumns,
                    CssClass = formFieldAttr.CssClass ?? "",
                    DataList = formFieldAttr.DataList ?? "",
                    Order = formFieldAttr.Order,
                    Section = formFieldAttr.Section ?? "Geral",
                    ShouldDisplay = shouldDisplay,
                    ConditionalDisplayRule = displayRule,
                    IsConditionallyRequired = isConditionallyRequired,
                    ConditionalRequiredRule = requiredRule,
                    ConditionalRequiredMessage = requiredMessage,
                    ReferenceFilters = referenceFilters,
                    Options = formFieldAttr.Type == EnumFieldType.Select ? GetSelectOptions(property.Name) : null,
                    FileUrl = fileUrl,
                    FileName = fileName,
                    FilePath = filePath,
                    ImageSize = formFieldAttr.Type == EnumFieldType.Image ? (formFieldAttr.ImageSize ?? "150x150") : null
                };
            }

            return null;
        }

        // NOVO MÉTODO: Converter valor para string formatada
        private string? FormatFieldValueToString(object? rawValue, EnumFieldType fieldType, PropertyInfo property)
        {
            if (rawValue == null)
            {
                return null;
            }

            // Para campos Date, formatar como yyyy-MM-dd (formato HTML5)
            if (fieldType == EnumFieldType.Date && rawValue is DateTime dateValue)
            {
                return dateValue.ToString("yyyy-MM-dd");
            }

            // Para campos DateTime, formatar como yyyy-MM-ddTHH:mm (formato HTML5)
            if (fieldType == EnumFieldType.DateTime && rawValue is DateTime dateTimeValue)
            {
                return dateTimeValue.ToString("yyyy-MM-ddTHH:mm");
            }

            // Para campos Time, formatar como HH:mm (formato HTML5)
            if (fieldType == EnumFieldType.Time)
            {
                if (rawValue is DateTime timeValue)
                {
                    return timeValue.ToString("HH:mm");
                }
                if (rawValue is TimeSpan timeSpanValue)
                {
                    return timeSpanValue.ToString(@"hh\:mm");
                }
            }

            // Para campos Select (Enums), converter para o valor string do enum
            if (fieldType == EnumFieldType.Select)
            {
                var propertyType = property.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(propertyType);

                if (propertyType.IsEnum || (underlyingType?.IsEnum ?? false))
                {
                    return Convert.ToInt32(rawValue).ToString();
                }

                return rawValue.ToString();
            }

            // Para campos Reference, converter para string (ID da entidade)
            if (fieldType == EnumFieldType.Reference)
            {
                if (rawValue is long longValue)
                {
                    return longValue == 0 ? "" : longValue.ToString();
                }
                if (rawValue is int intValue)
                {
                    return intValue == 0 ? "" : intValue.ToString();
                }

                return rawValue.ToString();
            }

            // Para campos Checkbox (bool), retornar "true" ou "false"
            if (fieldType == EnumFieldType.Checkbox)
            {
                return (rawValue is bool boolValue && boolValue).ToString().ToLower();
            }

            // Para campos Currency, formatar como decimal sem símbolo
            if (fieldType == EnumFieldType.Currency && rawValue is decimal decimalValue)
            {
                return decimalValue.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            }

            // Para campos Decimal
            if (fieldType == EnumFieldType.Decimal && rawValue is decimal decValue)
            {
                return decValue.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            }

            // Para outros tipos, retornar string
            return rawValue.ToString();
        }

        private static string? GetReferenceDisplayText(T entity, PropertyInfo property, Type? referenceType)
        {
            if (referenceType == null)
            {
                return null;
            }

            try
            {
                // Buscar a navigation property (ex: IdVeiculoMarca -> VeiculoMarca)
                var navigationPropertyName = property.Name.StartsWith("Id")
                    ? property.Name.Substring(2)
                    : property.Name;

                var navigationProperty = typeof(T).GetProperty(navigationPropertyName);

                if (navigationProperty == null)
                {
                    Console.WriteLine($"Navigation property não encontrada: {navigationPropertyName}");
                    return null;
                }

                // Obter a entidade relacionada
                var relatedEntity = navigationProperty.GetValue(entity);
                if (relatedEntity == null)
                {
                    Console.WriteLine($"Entidade relacionada é null para: {navigationPropertyName}");
                    return null;
                }

                // Buscar propriedade com [ReferenceText] ou [GridMain]
                var displayProperty = referenceType.GetProperties()
                    .FirstOrDefault(p =>
                        p.GetCustomAttributes(typeof(ReferenceTextAttribute), false).Any() ||
                        p.GetCustomAttributes(typeof(GridMainAttribute), false).Any());

                if (displayProperty == null)
                {
                    // Fallback: tentar Nome, Descricao ou qualquer string
                    displayProperty = referenceType.GetProperty("Nome") ??
                                    referenceType.GetProperty("Descricao") ??
                                    referenceType.GetProperties()
                                        .FirstOrDefault(p => p.PropertyType == typeof(string) && p.Name != "Id");
                }

                if (displayProperty != null)
                {
                    var value = displayProperty.GetValue(relatedEntity);
                    var displayText = value?.ToString();

                    Console.WriteLine($"DisplayText para {property.Name}: {displayText}");

                    return displayText;
                }

                Console.WriteLine($"DisplayProperty não encontrada para tipo: {referenceType.Name}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter DisplayText para {property.Name}: {ex.Message}");
                return null;
            }
        }

        private object? FormatFieldValue(object? rawValue, EnumFieldType fieldType, PropertyInfo property)
        {
            if (rawValue == null)
            {
                return null;
            }

            // Para campos Date, formatar como yyyy-MM-dd (formato HTML5)
            if (fieldType == EnumFieldType.Date && rawValue is DateTime dateValue)
            {
                return dateValue.ToString("yyyy-MM-dd");
            }

            // Para campos DateTime, formatar como yyyy-MM-ddTHH:mm (formato HTML5)
            if (fieldType == EnumFieldType.DateTime && rawValue is DateTime dateTimeValue)
            {
                return dateTimeValue.ToString("yyyy-MM-ddTHH:mm");
            }

            // Para campos Time, formatar como HH:mm (formato HTML5)
            if (fieldType == EnumFieldType.Time && rawValue is DateTime timeValue)
            {
                return timeValue.ToString("HH:mm");
            }

            if (fieldType == EnumFieldType.Time && rawValue is TimeSpan timeSpanValue)
            {
                return timeSpanValue.ToString(@"hh\:mm");
            }

            // Para campos Select (Enums), converter para o valor string do enum
            if (fieldType == EnumFieldType.Select)
            {
                var propertyType = property.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(propertyType);

                if (propertyType.IsEnum || (underlyingType?.IsEnum ?? false))
                {
                    return Convert.ToInt32(rawValue).ToString();
                }

                return rawValue.ToString();
            }

            // Para campos Reference, converter para string (ID da entidade)
            if (fieldType == EnumFieldType.Reference)
            {
                if (rawValue is long longValue && longValue == 0)
                {
                    return null;
                }
                if (rawValue is int intValue && intValue == 0)
                {
                    return null;
                }

                return rawValue.ToString();
            }

            // Para campos Checkbox (bool), garantir que seja bool
            if (fieldType == EnumFieldType.Checkbox)
            {
                return rawValue is bool boolValue && boolValue;
            }

            // Para campos Currency, formatar como decimal sem símbolo
            if (fieldType == EnumFieldType.Currency && rawValue is decimal decimalValue)
            {
                return decimalValue.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            }

            // Para outros tipos, retornar o valor original
            return rawValue;
        }

        #endregion

        #region Helpers para aplicar filtros

        /// <summary>
        /// Helper para aplicar filtros de data range
        /// </summary>
        protected IQueryable<T> ApplyDateRangeFilter<TProperty>(IQueryable<T> query, Dictionary<string, object> filters, string filterName, Expression<Func<T, TProperty>> propertyExpression)
        {
            var inicioKey = $"{filterName}_inicio";
            var fimKey = $"{filterName}_fim";

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (filters.ContainsKey(inicioKey) && DateTime.TryParse(filters[inicioKey].ToString(), out var inicio))
            {
                dataInicio = inicio;
            }

            if (filters.ContainsKey(fimKey) && DateTime.TryParse(filters[fimKey].ToString(), out var fim))
            {
                dataFim = fim.Date.AddDays(1).AddTicks(-1); // Fim do dia
            }

            if (dataInicio.HasValue || dataFim.HasValue)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, ((MemberExpression)propertyExpression.Body).Member.Name);

                Expression? condition = null;

                if (dataInicio.HasValue)
                {
                    var inicioConstant = Expression.Constant(dataInicio.Value);
                    var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, inicioConstant);
                    condition = condition == null ? greaterThanOrEqual : Expression.AndAlso(condition, greaterThanOrEqual);
                }

                if (dataFim.HasValue)
                {
                    var fimConstant = Expression.Constant(dataFim.Value);
                    var lessThanOrEqual = Expression.LessThanOrEqual(property, fimConstant);
                    condition = condition == null ? lessThanOrEqual : Expression.AndAlso(condition, lessThanOrEqual);
                }

                if (condition != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);
                    query = query.Where(lambda);
                }
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros de texto com múltiplas propriedades
        /// </summary>
        protected IQueryable<T> ApplyTextFilter(IQueryable<T> query, string searchTerm, params Expression<Func<T, string?>>[] properties)
        {
            if (string.IsNullOrEmpty(searchTerm) || properties.Length == 0)
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? condition = null;
            var searchTermLower = searchTerm.ToLower();

            foreach (var propertyExpr in properties)
            {
                var propertyName = ((MemberExpression)propertyExpr.Body).Member.Name;
                var property = Expression.Property(parameter, propertyName);

                // Verificar se não é null
                var notNull = Expression.NotEqual(property, Expression.Constant(null));

                // Aplicar ToLower na propriedade
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var propertyToLower = Expression.Call(property, toLowerMethod!);

                // Aplicar Contains com termo já em lower case
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                var searchConstant = Expression.Constant(searchTermLower);
                var containsCall = Expression.Call(propertyToLower, containsMethod!, searchConstant);

                // Combinar null check com contains
                var propertyCondition = Expression.AndAlso(notNull, containsCall);

                condition = condition == null ? propertyCondition : Expression.OrElse(condition, propertyCondition);
            }

            if (condition != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros de enum
        /// </summary>
        protected IQueryable<T> ApplyEnumFilter<TEnum>(IQueryable<T> query, Dictionary<string, object> filters, string filterName, Expression<Func<T, TEnum>> propertyExpression) where TEnum : struct, Enum
        {
            if (filters.TryGetValue(filterName, out var value) && Enum.TryParse<TEnum>(value.ToString(), out var enumValue))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, ((MemberExpression)propertyExpression.Body).Member.Name);
                var constant = Expression.Constant(enumValue);
                var equal = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros numéricos
        /// </summary>
        protected IQueryable<T> ApplyNumericFilter<TProperty>(IQueryable<T> query, Dictionary<string, object> filters, string filterName, Expression<Func<T, TProperty>> propertyExpression) where TProperty : struct, IComparable<TProperty>
        {
            if (filters.TryGetValue(filterName, out var value))
            {
                if (TryConvertToNumeric(value.ToString(), out TProperty numericValue))
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.Property(parameter, ((MemberExpression)propertyExpression.Body).Member.Name);
                    var constant = Expression.Constant(numericValue);
                    var equal = Expression.Equal(property, constant);
                    var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

                    query = query.Where(lambda);
                }
            }

            return query;
        }

        #endregion Helpers para aplicar filtros

        #region Criação via modal para campo referência

        /// <summary>
        /// Manipula criação via modal para campos de referência
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="controller">Controller</param>
        /// <param name="entity">Entidade a ser criada</param>
        /// <returns>ActionResult apropriado (JSON para AJAX, View para navegação normal)</returns>
        public static async Task<IActionResult> HandleModalCreate(StandardGridController<T> controller, T entity)
        {
            // Verifica se é requisição AJAX (modal)
            if (controller.Request.Headers.TryGetValue("X-Requested-With", out var value) && value == "XMLHttpRequest")
            {
                try
                {
                    if (controller.ModelState.IsValid)
                    {
                        // Executar lógica de criação
                        await controller.BeforeCreate(entity);
                        controller._context.Set<T>().Add(entity);
                        await controller._context.SaveChangesAsync();
                        await controller.AfterCreate(entity);

                        // Retornar JSON para o modal
                        return controller.Json(new
                        {
                            success = true,
                            id = entity.Id,
                            text = GetDisplayText(entity),
                            name = GetDisplayText(entity), // Compatibilidade
                            message = "Registro criado com sucesso!"
                        });
                    }
                    else
                    {
                        // Retornar erros de validação
                        var errors = controller.ModelState
                            .Where(x => x.Value?.Errors.Count > 0)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => string.Join("; ", kvp.Value?.Errors.Select(e => e.ErrorMessage) ?? [])
                            );

                        return controller.Json(new
                        {
                            success = false,
                            errors
                        });
                    }
                }
                catch (Exception ex)
                {
                    return controller.Json(new
                    {
                        success = false,
                        errors = new Dictionary<string, string>
                        {
                            ["general"] = $"Erro interno: {ex.Message}"
                        }
                    });
                }
            }

            // Comportamento normal se não for AJAX
            return await DefaultCreate(controller, entity);
        }

        /// <summary>
        /// Execução padrão do Create (comportamento original)
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="controller">Controller</param>
        /// <param name="entity">Entidade</param>
        /// <returns>ActionResult</returns>
        private static async Task<IActionResult> DefaultCreate(StandardGridController<T> controller, T entity)
        {
            try
            {
                await controller.BeforeCreate(entity);

                if (controller.ModelState.IsValid)
                {
                    controller._context.Set<T>().Add(entity);
                    await controller._context.SaveChangesAsync();

                    await controller.AfterCreate(entity);
                    return controller.RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                //controller.Logger?.LogError(ex, "Erro ao criar {EntityType}", typeof(T).Name);
                controller.ModelState.AddModelError("", $"Erro ao salvar: {ex.Message}");
            }

            // Se chegou até aqui, há erros - mostrar form novamente
            var viewModel = await controller.BuildFormViewModelAsync(entity, "Create");
            return controller.View(viewModel);
        }

        /// <summary>
        /// Obtém o texto de exibição apropriado para uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="entity">Instância da entidade</param>
        /// <returns>Texto para exibição</returns>
        private static string GetDisplayText(T entity)
        {
            var type = typeof(T);

            // Propriedades comuns para exibição, em ordem de prioridade
            var displayProperties = new[] { "Nome", "Descricao", "Titulo", "RazaoSocial", "Codigo", "Numero" };

            foreach (var propName in displayProperties)
            {
                var prop = type.GetProperty(propName);
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    var value = prop.GetValue(entity) as string;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }
            }

            // Fallback para nome do tipo + ID
            var typeName = type.Name;
            return $"{typeName} #{entity.Id}";
        }

        #endregion

        protected async Task<TabContentViewModel> BuildTabContentViewModelAsync(
            string tabId,
            string title,
            string icon,
            long parentId,
            string parentController,
            Dictionary<string, object>? filters = null)
        {
            var query = GetBaseQuery();

            // Aplicar filtros se fornecidos
            if (filters != null && filters.Count != 0)
            {
                query = ApplyFilters(query, filters);
            }

            var items = await query.ToListAsync();

            // Obter colunas da grid
            var columns = GetGridColumns();

            return new TabContentViewModel
            {
                TabId = tabId,
                Title = title,
                Icon = icon,
                ControllerName = typeof(T).Name.Replace("Controller", ""),
                ParentId = parentId,
                ParentController = parentController,
                Items = [.. items.Cast<object>()],
                Columns = columns,
                CanCreate = CanCreate(null),
                CanEdit = true,
                CanDelete = true
            };
        }

        protected List<TabColumnDefinition> GetGridColumns()
        {
            var columns = new List<TabColumnDefinition>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                // GridMain
                var gridMainAttr = property.GetCustomAttribute<GridMainAttribute>();
                if (gridMainAttr != null)
                {
                    columns.Add(new TabColumnDefinition
                    {
                        PropertyName = property.Name,
                        DisplayName = gridMainAttr.DisplayName,
                        Width = gridMainAttr.Width,
                        Order = gridMainAttr.Order
                    });
                    continue;
                }

                // GridField
                var gridFieldAttr = property.GetCustomAttribute<GridFieldAttribute>();
                if (gridFieldAttr != null)
                {
                    columns.Add(new TabColumnDefinition
                    {
                        PropertyName = property.Name,
                        DisplayName = gridFieldAttr.DisplayName,
                        Width = gridFieldAttr.Width,
                        Format = gridFieldAttr.Format,
                        Order = gridFieldAttr.Order
                    });
                    continue;
                }

                // GridId
                var gridIdAttr = property.GetCustomAttribute<GridIdAttribute>();
                if (gridIdAttr != null)
                {
                    columns.Add(new TabColumnDefinition
                    {
                        PropertyName = property.Name,
                        DisplayName = "Código",
                        Width = "80px",
                        Order = 0
                    });
                }
            }

            return [.. columns.OrderBy(c => c.Order)];
        }
    }
}