using AutoGestao.Attributes;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Helpers;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoGestao.Controllers
{
    public abstract class StandardGridController<T>(ApplicationDbContext context) : Controller() where T : BaseEntidade, new()
    {
        protected readonly ApplicationDbContext _context = context;

        protected virtual StandardGridViewModel ConfigureGrid()
        {
            var retorno = new StandardGridViewModel($"{typeof(T).Name}s", $"Gerencie todos os {typeof(T).Name}s", $"{typeof(T).Name}s")
            {
                Columns =
                [
                    new() { Name = "Id", DisplayName = "Cód", Type = EnumGridColumnType.Text, Sortable = true, Width = "65px" },
                    new() { Name = "Actions", DisplayName = "Ações", Type = EnumGridColumnType.Actions, Sortable = false, Width = "100px" }
                ]
            };

            return retorno;
        }

        protected virtual IQueryable<T> GetBaseQuery()
        {
            return _context.Set<T>().AsQueryable();
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
            // Primeiro ajusta automaticamente se for Enum
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

        #region Métodos Virtuais para Formulários (podem ser sobrescritos)

        /// <summary>
        /// Customizar campos do formulário baseado na action
        /// </summary>
        protected virtual void ConfigureFormFields(List<FormFieldViewModel> fields, T entity, string action)
        {
        }

        protected virtual bool CanCreate(T entity)
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

        protected virtual Task BeforeCreate(T entity)
        {
            entity.DataCadastro = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        protected virtual Task AfterCreate(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task BeforeUpdate(T entity)
        {
            entity.DataAlteracao = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        protected virtual Task AfterUpdate(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task BeforeDelete(T entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterDelete(T entity)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Actions para Formulários Dinâmicos

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
            var gridConfig = ConfigureGrid();
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
            var gridConfig = ConfigureGrid();
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
        public virtual IActionResult Create()
        {
            var entity = new T();
            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>();

            if (!CanCreate(entity))
            {
                TempData["Error"] = "Você não tem permissão para cadastrar um novo registro.";
                return Forbid();
            }

            PopulateEnumsInViewBag();

            var viewModel = BuildFormViewModel(entity, "Create");

            if (IsAjaxRequest())
            {
                // Tenta encontrar view parcial específica primeiro
                var partialViewName = $"_Create{typeof(T).Name}Form";
                if (ViewExists(partialViewName))
                {
                    return PartialView(partialViewName, viewModel);
                }

                // Fallback para view parcial genérica
                return PartialView("_CreateForm", viewModel);
            }

            if (formTabs?.EnableTabs == true)
            {
                viewModel = BuildTabbedFormViewModel(entity, "Create");
                return View("_TabbedForm", viewModel);
            }
            else
            {
                return View("_StandardForm", viewModel);
            }
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
                TempData["Error"] = "Você não tem permissão para cadastrar um novo registro.";
                return Forbid();
            }

            if (IsAjaxRequest())
            {
                return await HandleModalCreate(this, entity);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await BeforeCreate(entity);
                    _context.Set<T>().Add(entity);
                    await _context.SaveChangesAsync();
                    await AfterCreate(entity);

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Registro criado com sucesso!", redirectUrl = Url.Action("Index") });
                    }

                    TempData["Success"] = "Registro criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar registro: {ex.Message}");
                }
            }

            var viewModel = BuildFormViewModel(entity, "Create");
            AddModelStateToViewModel(viewModel);

            return Request.IsAjaxRequest() 
                ? PartialView("_StandardFormContent", viewModel)
                : View("_StandardForm", viewModel);
        }

        /// <summary>
        /// GET: Edit - Exibir formulário de edição
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Edit(int id)
        {
            var entity = await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            if (!CanEdit(entity))
            {
                TempData["Error"] = "Você não tem permissão para editar este registro.";
                return RedirectToAction(nameof(Index));
            }

            PopulateEnumsInViewBag();
            
            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>();
            if (formTabs?.EnableTabs == true)
            {
                var viewModel = BuildTabbedFormViewModel(entity, "Edit");
                return View("_TabbedForm", viewModel);
            }
            else
            {
                var viewModel = BuildFormViewModel(entity, "Edit");
                return View("_StandardForm", viewModel);
            }
        }

        /// <summary>
        /// POST: Edit - Processar edição da entidade
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, T entity)
        {
            if (id != entity.Id)
            {
                return NotFound();
            }

            var existingEntity = await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id);
            if (existingEntity == null)
            {
                return NotFound();
            }

            if (!CanEdit(existingEntity))
            {
                TempData["Error"] = "Você não tem permissão para editar este registro.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualizar propriedades
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);

                    await BeforeUpdate(existingEntity);
                    await _context.SaveChangesAsync();
                    await AfterUpdate(existingEntity);

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Registro atualizado com sucesso!", redirectUrl = Url.Action("Index") });
                    }

                    TempData["Success"] = "Registro atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao atualizar registro: {ex.Message}");
                }
            }

            var viewModel = BuildFormViewModel(entity, "Edit");
            AddModelStateToViewModel(viewModel);

            return Request.IsAjaxRequest() ? PartialView("_StandardFormContent", viewModel) : View("_StandardForm", viewModel);
        }

        /// <summary>
        /// GET: Details - Exibir detalhes da entidade
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Details(int id)
        {
            var entity = await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            PopulateEnumsInViewBag();

            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>();
            if (formTabs?.EnableTabs == true)
            {
                var viewModel = BuildTabbedFormViewModel(entity, "Details");
                return View("_TabbedForm", viewModel);
            }
            else
            {
                var viewModel = BuildFormViewModel(entity, "Details");
                return View("_StandardForm", viewModel);
            }
        }

        /// <summary>
        /// POST: Delete - Excluir entidade
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Delete(int id)
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
                    return Json(new { success = false, message = "Você não tem permissão para excluir este registro." });
                }
                TempData["Error"] = "Você não tem permissão para excluir este registro.";
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
                    return Json(new { success = true, message = "Registro excluído com sucesso!" });
                }

                TempData["Success"] = "Registro excluído com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { success = false, message = $"Erro ao excluir registro: {ex.Message}" });
                }
                TempData["Error"] = $"Erro ao excluir registro: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

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

        /// <summary>
        /// Renderizar conteúdo de uma aba específica
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> RenderTab(int id, string tabId)
        {
            var entity = await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            var tabs = ConfigureTabs(entity);
            var tab = tabs.FirstOrDefault(t => t.TabId == tabId);

            if (tab == null || !tab.HasAccess)
            {
                return Forbid();
            }

            if (tabId == "principal")
            {
                // Renderizar formulário principal
                var viewModel = BuildFormViewModel(entity, "Details");
                return PartialView("_StandardFormContent", viewModel);
            }

            // Renderizar aba personalizada
            return await RenderCustomTab(entity, tab);
        }

        /// <summary>
        /// Renderizar aba personalizada (override em controllers específicos)
        /// </summary>
        protected virtual Task<IActionResult> RenderCustomTab(T entity, FormTabViewModel tab)
        {
            // Implementação padrão - pode ser sobrescrita
            return Task.FromResult<IActionResult>(PartialView($"_Tab{tab.TabId}", entity));
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Constrói o ViewModel do formulário baseado nos atributos da entidade
        /// </summary>
        public StandardFormViewModel BuildFormViewModel(T entity, string action)
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

            // Agrupar campos por seção
            var fieldsBySection = properties
                .Select(p => CreateFieldFromProperty(p, entity, action))
                .Where(f => f != null)
                .GroupBy(f => f.Section ?? "Não Informado")
                .ToList();

            foreach (var sectionGroup in fieldsBySection)
            {
                var section = new FormSectionViewModel
                {
                    Name = sectionGroup.Key,
                    Icon = GetSectionIcon(sectionGroup.Key),
                    Fields = [.. sectionGroup.OrderBy(f => f.Order)]
                };

                // Determinar quantas colunas a seção deve ter
                section.GridColumns = section.Fields.Count != 0 ? section.Fields.Max(f => f.GridColumns) : 1;

                viewModel.Sections.Add(section);
            }

            // Permitir customizações específicas do controller
            var allFields = viewModel.Sections.SelectMany(s => s.Fields).ToList();
            ConfigureFormFields(allFields, entity, action);

            return viewModel;
        }

        /// <summary>
        /// Obtém as propriedades que devem aparecer no formulário
        /// </summary>
        private static List<PropertyInfo> GetFormProperties()
        {
            return [.. typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => !IsIgnoredProperty(p))
                .OrderBy(p => GetPropertyOrder(p))];
        }

        /// <summary>
        /// Determina se uma propriedade deve ser auto-gerada no formulário
        /// </summary>
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

        private static bool TryConvertToNumeric<TProperty>(string value, out TProperty result)
        {
            result = default(TProperty);

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

        private static int GetPropertyOrder(PropertyInfo property)
        {
            var formField = property.GetCustomAttribute<FormFieldAttribute>();
            if (formField != null)
            {
                return formField.Order;
            }

            var display = property.GetCustomAttribute<DisplayAttribute>();
            if (display?.Order != null)
            {
                return display.Order;
            }

            // Ordem padrão baseada no nome
            return property.Name.ToLower() switch
            {
                var name when name.Contains("nome") => 1,
                var name when name.Contains("codigo") => 2,
                var name when name.Contains("tipo") => 3,
                var name when name.Contains("email") => 50,
                var name when name.Contains("telefone") => 51,
                var name when name.Contains("endereco") => 100,
                var name when name.Contains("observa") => 1000,
                _ => 500
            };
        }

        private static string GetDisplayName(PropertyInfo property)
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
                EnumFieldType.Phone => "(00) 00000-0000",
                EnumFieldType.Cpf => "000.000.000-00",
                EnumFieldType.Cnpj => "00.000.000/0000-00",
                EnumFieldType.Cep => "00000-000",
                EnumFieldType.Currency => "R$ 0,00",
                EnumFieldType.Date => "dd/mm/aaaa",
                EnumFieldType.TextArea => $"Digite as {GetDisplayName(property).ToLower()}...",
                _ => $"Digite {GetDisplayName(property).ToLower()}"
            };
        }

        private static bool IsRequiredProperty(PropertyInfo property)
        {
            if (property.GetCustomAttribute<RequiredAttribute>() != null)
            {
                return true;
            }

            // Convenção: tipos não-nullable são obrigatórios (exceto string)
            var type = property.PropertyType;
            return !type.IsClass && Nullable.GetUnderlyingType(type) == null;
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

        private void AddModelStateToViewModel(StandardFormViewModel viewModel)
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

        /// <summary>
        /// Construir ViewModel para formulário com abas
        /// </summary>
        private TabbedFormViewModel BuildTabbedFormViewModel(T entity, string action)
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
                ActiveTab = formTabs.DefaultTab ?? "principal"
            };

            // Configurar abas
            viewModel.Tabs = ConfigureTabs(entity);

            // Construir formulário principal para a primeira aba
            var mainFormViewModel = BuildFormViewModel(entity, action);
            viewModel.Sections = mainFormViewModel.Sections;
            viewModel.ModelState = mainFormViewModel.ModelState;

            return viewModel;
        }

        private bool IsAjaxRequest()
        {
            return Request.Headers.ContainsKey("X-Requested-With") ||
                   Request.Query.ContainsKey("ajax") ||
                   Request.ContentType?.Contains("application/json") == true;
        }

        private bool ViewExists(string viewName)
        {
            var viewEngine = HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine))
                as Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine;

            var viewResult = viewEngine?.FindView(ControllerContext, viewName, false);
            return viewResult?.Success ?? false;
        }

        #endregion

        #region Automated Enum Detection and Population

        /// <summary>
        /// Determina automaticamente o tipo de campo baseado na propriedade (MODIFICADO)
        /// </summary>
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

        /// <summary>
        /// Obtém automaticamente opções para propriedades Enum
        /// </summary>
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

        /// <summary>
        /// CreateFieldFromProperty para usar automação
        /// </summary>
        private FormFieldViewModel? CreateFieldFromProperty(PropertyInfo property, T entity, string action)
        {
            var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();

            if (formFieldAttr != null)
            {
                // ============================================================
                // PROCESSAMENTO DE REGRAS CONDICIONAIS
                // ============================================================

                // Obtém as novas anotações de regras condicionais
                var conditionalDisplayAttr = property.GetCustomAttribute<ConditionalDisplayAttribute>();
                var conditionalRequiredAttr = property.GetCustomAttribute<ConditionalRequiredAttribute>();

                // Regra de exibição (usa nova anotação ou fallback para sistema antigo)
                var displayRule = "";
                if (conditionalDisplayAttr != null)
                {
                    displayRule = conditionalDisplayAttr.Rule;
                }

                // Avalia se o campo deve ser exibido
                var shouldDisplay = true;
                if (!string.IsNullOrEmpty(displayRule))
                {
                    shouldDisplay = ConditionalExpressionEvaluator.Evaluate(displayRule, entity, typeof(T));
                }

                // Regra de obrigatoriedade condicional
                var requiredRule = conditionalRequiredAttr?.Rule ?? "";
                var isConditionallyRequired = false;
                var requiredMessage = conditionalRequiredAttr?.ErrorMessage ?? "";

                if (!string.IsNullOrEmpty(requiredRule))
                {
                    isConditionallyRequired = ConditionalExpressionEvaluator.Evaluate(requiredRule, entity, typeof(T));
                }

                // Determina se o campo é obrigatório
                var isRequired = formFieldAttr.Required || isConditionallyRequired;

                return new FormFieldViewModel
                {
                    PropertyName = property.Name,
                    DisplayName = formFieldAttr.Name ?? GetDisplayName(property),
                    Icon = formFieldAttr.Icon ?? GetDefaultIcon(property),
                    Placeholder = formFieldAttr.Placeholder ?? GetDefaultPlaceholder(property),
                    Type = formFieldAttr.Type,
                    Required = isRequired,
                    ReadOnly = action == "Details" || formFieldAttr.ReadOnly,
                    Value = property.GetValue(entity),
                    Reference = formFieldAttr.Reference ?? null,
                    ValidationRegex = formFieldAttr.ValidationRegex ?? "",
                    ValidationMessage = formFieldAttr.ValidationMessage ?? "",

                    // Novas regras condicionais
                    ConditionalDisplayRule = displayRule,
                    ConditionalRequiredRule = requiredRule,
                    ConditionalRequiredMessage = requiredMessage,
                    ShouldDisplay = shouldDisplay,
                    IsConditionallyRequired = isConditionallyRequired,
                    GridColumns = formFieldAttr.GridColumns,
                    CssClass = formFieldAttr.CssClass ?? "",
                    DataList = formFieldAttr.DataList ?? "",
                    Order = formFieldAttr.Order,
                    Section = formFieldAttr.Section ?? "Não Informado",
                    Options = formFieldAttr.Type == EnumFieldType.Select
                        ? GetSelectOptions(property.Name)
                        : []
                };
            }

            return null;
        }

        // <summary>
        // Popula automaticamente a ViewBag com todos os Enums da entidade
        // </summary>
        protected void PopulateEnumsInViewBag()
        {
            var enumProperties = typeof(T).GetProperties()
                .Where(p => {
                    var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    return type.IsEnum;
                });

            foreach (var property in enumProperties)
            {
                var options = StandardGridController<T>.GetAutoEnumOptions(property.Name);
                if (options.Count != 0)
                {
                    ViewBag.GetType().GetProperty(property.Name)?.SetValue(ViewBag, options);
                    // Alternativamente, use ViewData:
                    ViewData[property.Name] = options;
                }
            }
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
            if (filters.TryGetValue(filterName, out var value) && Enum.TryParse<TEnum>(value.ToString(), out TEnum enumValue))
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

        /// <summary>
        /// Manipula criação via modal para campos de referência
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="controller">Controller</param>
        /// <param name="entity">Entidade a ser criada</param>
        /// <returns>ActionResult apropriado (JSON para AJAX, View para navegação normal)</returns>
        public static async Task<IActionResult> HandleModalCreate<T>(StandardGridController<T> controller, T entity) where T : BaseEntidade, new()
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
                                kvp => string.Join("; ", kvp.Value?.Errors.Select(e => e.ErrorMessage) ?? new string[0])
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
        private static async Task<IActionResult> DefaultCreate<T>(StandardGridController<T> controller, T entity) where T : BaseEntidade, new()
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
            var viewModel = controller.BuildFormViewModel(entity, "Create");
            return controller.View(viewModel);
        }

        /// <summary>
        /// Obtém o texto de exibição apropriado para uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="entity">Instância da entidade</param>
        /// <returns>Texto para exibição</returns>
        private static string GetDisplayText<T>(T entity) where T : BaseEntidade
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
    }
}