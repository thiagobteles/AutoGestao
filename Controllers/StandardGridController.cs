using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AutoGestao.Controllers
{
    public abstract class StandardGridController<T>(ApplicationDbContext context) : Controller() where T : BaseEntidade, new()
    {
        protected readonly ApplicationDbContext _context = context;
        protected abstract IQueryable<T> GetBaseQuery();
        protected abstract StandardGridViewModel ConfigureGrid();
        protected abstract IQueryable<T> ApplyFilters(IQueryable<T> query, Dictionary<string, object> filters);
        protected abstract IQueryable<T> ApplySort(IQueryable<T> query, string orderBy, string orderDirection);

        /// <summary>
        /// Helper para aplicar filtros de data range
        /// </summary>
        protected IQueryable<T> ApplyDateRangeFilter<TProperty>(
            IQueryable<T> query,
            Dictionary<string, object> filters,
            string filterName,
            System.Linq.Expressions.Expression<Func<T, TProperty>> propertyExpression)
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
                var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                var property = System.Linq.Expressions.Expression.Property(parameter, ((System.Linq.Expressions.MemberExpression)propertyExpression.Body).Member.Name);

                System.Linq.Expressions.Expression? condition = null;

                if (dataInicio.HasValue)
                {
                    var inicioConstant = System.Linq.Expressions.Expression.Constant(dataInicio.Value);
                    var greaterThanOrEqual = System.Linq.Expressions.Expression.GreaterThanOrEqual(property, inicioConstant);
                    condition = condition == null ? greaterThanOrEqual : System.Linq.Expressions.Expression.AndAlso(condition, greaterThanOrEqual);
                }

                if (dataFim.HasValue)
                {
                    var fimConstant = System.Linq.Expressions.Expression.Constant(dataFim.Value);
                    var lessThanOrEqual = System.Linq.Expressions.Expression.LessThanOrEqual(property, fimConstant);
                    condition = condition == null ? lessThanOrEqual : System.Linq.Expressions.Expression.AndAlso(condition, lessThanOrEqual);
                }

                if (condition != null)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(condition, parameter);
                    query = query.Where(lambda);
                }
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros de texto com múltiplas propriedades
        /// </summary>
        protected IQueryable<T> ApplyTextFilter(
            IQueryable<T> query,
            string searchTerm,
            params System.Linq.Expressions.Expression<Func<T, string?>>[] properties)
        {
            if (string.IsNullOrEmpty(searchTerm) || properties.Length == 0)
            {
                return query;
            }

            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
            System.Linq.Expressions.Expression? condition = null;

            foreach (var propertyExpr in properties)
            {
                var propertyName = ((System.Linq.Expressions.MemberExpression)propertyExpr.Body).Member.Name;
                var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);

                // Verificar se não é null
                var notNull = System.Linq.Expressions.Expression.NotEqual(property, System.Linq.Expressions.Expression.Constant(null));

                // Aplicar Contains
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchConstant = System.Linq.Expressions.Expression.Constant(searchTerm);
                var containsCall = System.Linq.Expressions.Expression.Call(property, containsMethod!, searchConstant);

                // Combinar null check com contains
                var propertyCondition = System.Linq.Expressions.Expression.AndAlso(notNull, containsCall);

                condition = condition == null ? propertyCondition : System.Linq.Expressions.Expression.OrElse(condition, propertyCondition);
            }

            if (condition != null)
            {
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(condition, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros de enum
        /// </summary>
        protected IQueryable<T> ApplyEnumFilter<TEnum>(
            IQueryable<T> query,
            Dictionary<string, object> filters,
            string filterName,
            System.Linq.Expressions.Expression<Func<T, TEnum>> propertyExpression) where TEnum : struct, Enum
        {
            if (filters.ContainsKey(filterName) &&
                Enum.TryParse<TEnum>(filters[filterName].ToString(), out TEnum enumValue))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                var property = System.Linq.Expressions.Expression.Property(parameter, ((System.Linq.Expressions.MemberExpression)propertyExpression.Body).Member.Name);
                var constant = System.Linq.Expressions.Expression.Constant(enumValue);
                var equal = System.Linq.Expressions.Expression.Equal(property, constant);
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equal, parameter);

                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Helper para aplicar filtros numéricos
        /// </summary>
        protected IQueryable<T> ApplyNumericFilter<TProperty>(
            IQueryable<T> query,
            Dictionary<string, object> filters,
            string filterName,
            System.Linq.Expressions.Expression<Func<T, TProperty>> propertyExpression) where TProperty : struct, IComparable<TProperty>
        {
            if (filters.ContainsKey(filterName))
            {
                var value = filters[filterName].ToString();
                if (TryConvertToNumeric(value, out TProperty numericValue))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
                    var property = System.Linq.Expressions.Expression.Property(parameter, ((System.Linq.Expressions.MemberExpression)propertyExpression.Body).Member.Name);
                    var constant = System.Linq.Expressions.Expression.Constant(numericValue);
                    var equal = System.Linq.Expressions.Expression.Equal(property, constant);
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equal, parameter);

                    query = query.Where(lambda);
                }
            }

            return query;
        }


        #region Métodos Virtuais para Formulários (podem ser sobrescritos)

        /// <summary>
        /// Configurar opções para campos select
        /// </summary>
        protected virtual List<SelectListItem> GetSelectOptions(string propertyName)
        {
            return [];
        }

        /// <summary>
        /// Verificar se pode editar a entidade
        /// </summary>
        protected virtual bool CanEdit(T entity)
        {
            return true;
        }

        /// <summary>
        /// Verificar se pode deletar a entidade
        /// </summary>
        protected virtual bool CanDelete(T entity)
        {
            return true;
        }

        /// <summary>
        /// Customizar campos do formulário baseado na action
        /// </summary>
        protected virtual void ConfigureFormFields(List<FormFieldViewModel> fields, T entity, string action)
        {
            // Override para customizações específicas
        }

        /// <summary>
        /// Executado antes de criar a entidade
        /// </summary>
        protected virtual Task BeforeCreate(T entity)
        {
            entity.DataCadastro = DateTime.Now;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executado após criar a entidade
        /// </summary>
        protected virtual Task AfterCreate(T entity)
        {
            // Override para ações pós-criação
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executado antes de atualizar a entidade
        /// </summary>
        protected virtual Task BeforeUpdate(T entity)
        {
            entity.DataAlteracao = DateTime.Now;
            return Task.CompletedTask;

        }

        /// <summary>
        /// Executado após atualizar a entidade
        /// </summary>
        protected virtual Task AfterUpdate(T entity)
        {
            // Override para ações pós-atualização
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executado antes de deletar a entidade
        /// </summary>
        protected virtual Task BeforeDelete(T entity)
        {
            // Override para validações antes da exclusão
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executado após deletar a entidade
        /// </summary>
        protected virtual Task AfterDelete(T entity)
        {
            // Override para ações pós-exclusão
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
            gridConfig.Items = items.Cast<object>().ToList();
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
        public virtual Task<IActionResult> Create()
        {
            var entity = new T();
            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>();

            if (formTabs?.EnableTabs == true)
            {
                var viewModel = BuildTabbedFormViewModel(entity, "Create");
                return Task.FromResult<IActionResult>(View("_TabbedForm", viewModel));
            }
            else
            {
                var viewModel = BuildFormViewModel(entity, "Create");
                return Task.FromResult<IActionResult>(View("_StandardForm", viewModel));
            }
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
        /// POST: Create - Processar criação da entidade
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(T entity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    BeforeCreate(entity);
                    _context.Set<T>().Add(entity);
                    await _context.SaveChangesAsync();
                    AfterCreate(entity);

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

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StandardFormContent", viewModel);
            }

            return View("_StandardForm", viewModel);
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

                    BeforeUpdate(existingEntity);
                    await _context.SaveChangesAsync();
                    AfterUpdate(existingEntity);

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

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StandardFormContent", viewModel);
            }

            return View("_StandardForm", viewModel);
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
                BeforeDelete(entity);
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                AfterDelete(entity);

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

        #region Métodos Privados para Construção do ViewModel


        /// <summary>
        /// Constrói o ViewModel do formulário baseado nos atributos da entidade
        /// </summary>
        private StandardFormViewModel BuildFormViewModel(T entity, string action)
        {
            var formConfig = typeof(T).GetCustomAttribute<FormConfigAttribute>() ?? new FormConfigAttribute();
            var properties = GetFormProperties();

            var viewModel = new StandardFormViewModel
            {
                Title = formConfig.Title ?? GetDefaultTitle(action),
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
                .GroupBy(f => f.Section ?? "Dados Básicos")
                .ToList();

            foreach (var sectionGroup in fieldsBySection)
            {
                var section = new FormSectionViewModel
                {
                    Name = sectionGroup.Key,
                    Icon = GetSectionIcon(sectionGroup.Key),
                    Fields = sectionGroup.OrderBy(f => f.Order).ToList()
                };

                // Determinar quantas colunas a seção deve ter
                section.GridColumns = section.Fields.Any() ? section.Fields.Max(f => f.GridColumns) : 1;

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
        private List<PropertyInfo> GetFormProperties()
        {
            return typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => !IsIgnoredProperty(p))
                .OrderBy(p => GetPropertyOrder(p))
                .ToList();
        }

        /// <summary>
        /// Cria um campo do formulário baseado na propriedade
        /// </summary>
        private FormFieldViewModel CreateFieldFromProperty(PropertyInfo property, T entity, string action)
        {
            var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();

            // Se tem atributo FormField, usar as configurações dele
            if (formFieldAttr != null)
            {
                return new FormFieldViewModel
                {
                    PropertyName = property.Name,
                    DisplayName = formFieldAttr.DisplayName ?? GetDisplayName(property),
                    Icon = formFieldAttr.Icon ?? GetDefaultIcon(property),
                    Placeholder = formFieldAttr.Placeholder ?? GetDefaultPlaceholder(property),
                    Type = formFieldAttr.Type,
                    Required = formFieldAttr.Required,
                    ReadOnly = formFieldAttr.ReadOnly || action == "Details",
                    Value = property.GetValue(entity),
                    ValidationRegex = formFieldAttr.ValidationRegex,
                    ValidationMessage = formFieldAttr.ValidationMessage,
                    ConditionalField = formFieldAttr.ConditionalField,
                    ConditionalValue = formFieldAttr.ConditionalValue,
                    GridColumns = formFieldAttr.GridColumns,
                    CssClass = formFieldAttr.CssClass,
                    DataList = formFieldAttr.DataList,
                    Order = formFieldAttr.Order,
                    Section = formFieldAttr.Section ?? "Dados Básicos",
                    Options = formFieldAttr.Type == FormFieldType.Select ? GetSelectOptions(property.Name) : []
                };
            }

            // Se não tem atributo, tentar gerar automaticamente
            if (StandardGridController<T>.ShouldAutoGenerateField(property))
            {
                return new FormFieldViewModel
                {
                    PropertyName = property.Name,
                    DisplayName = GetDisplayName(property),
                    Icon = GetDefaultIcon(property),
                    Placeholder = GetDefaultPlaceholder(property),
                    Type = DetermineFieldType(property),
                    Required = IsRequiredProperty(property),
                    ReadOnly = action == "Details",
                    Value = property.GetValue(entity),
                    Order = GetPropertyOrder(property),
                    Section = DetermineSection(property),
                    GridColumns = 1,
                    Options = DetermineFieldType(property) == FormFieldType.Select ? GetSelectOptions(property.Name) : []
                };
            }

            return null;
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
            var ignoredNames = new[] { "Id", "DataCadastro", "DataAlteracao", "UsuarioCadastro", "UsuarioAlteracao" };
            return !ignoredNames.Contains(property.Name);
        }

        /// <summary>
        /// Determina automaticamente o tipo de campo baseado na propriedade
        /// </summary>
        private FormFieldType DetermineFieldType(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var propertyName = property.Name.ToLower();

            // Verificar nome da propriedade primeiro
            if (propertyName.Contains("email"))
            {
                return FormFieldType.Email;
            }

            if (propertyName.Contains("telefone") || propertyName.Contains("celular"))
            {
                return FormFieldType.Phone;
            }

            if (propertyName == "cpf")
            {
                return FormFieldType.Cpf;
            }

            if (propertyName == "cnpj")
            {
                return FormFieldType.Cnpj;
            }

            if (propertyName == "cep")
            {
                return FormFieldType.Cep;
            }

            if (propertyName.Contains("senha") || propertyName.Contains("password"))
            {
                return FormFieldType.Password;
            }

            // Verificar tipo da propriedade
            if (type == typeof(bool))
            {
                return FormFieldType.Checkbox;
            }

            if (type.IsEnum)
            {
                return FormFieldType.Select;
            }

            if (type == typeof(DateTime))
            {
                return FormFieldType.Date;
            }

            if (type == typeof(decimal) && (propertyName.Contains("valor") || propertyName.Contains("preco")))
            {
                return FormFieldType.Currency;
            }

            if (IsNumericType(type))
            {
                return FormFieldType.Number;
            }

            if (type == typeof(string))
            {
                var maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLength?.Length > 255)
                {
                    return FormFieldType.TextArea;
                }
            }

            return FormFieldType.Text;
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
                if (values.ContainsKey(filter.Name))
                {
                    filter.Value = values[filter.Name];
                }

                // Para filtros de data range, verificar campos específicos
                if (filter.Type == GridFilterType.DateRange)
                {
                    var inicioKey = $"{filter.Name}_inicio";
                    var fimKey = $"{filter.Name}_fim";

                    if (values.ContainsKey(inicioKey))
                    {
                        ViewBag.GetType().GetProperty(inicioKey)?.SetValue(ViewBag, values[inicioKey]);
                    }

                    if (values.ContainsKey(fimKey))
                    {
                        ViewBag.GetType().GetProperty(fimKey)?.SetValue(ViewBag, values[fimKey]);
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

        private bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(long) || type == typeof(decimal) ||
                   type == typeof(double) || type == typeof(float) || type == typeof(short);
        }

        private bool IsIgnoredProperty(PropertyInfo property)
        {
            return property.GetCustomAttribute<FormFieldAttribute>() == null &&
                   !StandardGridController<T>.ShouldAutoGenerateField(property);
        }

        private int GetPropertyOrder(PropertyInfo property)
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

        private string GetDisplayName(PropertyInfo property)
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

        private string GetDefaultIcon(PropertyInfo property)
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

        private string GetDefaultPlaceholder(PropertyInfo property)
        {
            var propertyName = property.Name.ToLower();
            var fieldType = DetermineFieldType(property);

            return fieldType switch
            {
                FormFieldType.Email => "exemplo@email.com",
                FormFieldType.Phone => "(00) 00000-0000",
                FormFieldType.Cpf => "000.000.000-00",
                FormFieldType.Cnpj => "00.000.000/0000-00",
                FormFieldType.Cep => "00000-000",
                FormFieldType.Currency => "R$ 0,00",
                FormFieldType.Date => "dd/mm/aaaa",
                FormFieldType.TextArea => $"Digite as {GetDisplayName(property).ToLower()}...",
                _ => $"Digite {GetDisplayName(property).ToLower()}"
            };
        }

        private bool IsRequiredProperty(PropertyInfo property)
        {
            if (property.GetCustomAttribute<RequiredAttribute>() != null)
            {
                return true;
            }

            // Convenção: tipos não-nullable são obrigatórios (exceto string)
            var type = property.PropertyType;
            return !type.IsClass && Nullable.GetUnderlyingType(type) == null;
        }

        private string DetermineSection(PropertyInfo property)
        {
            var propertyName = property.Name.ToLower();

            return propertyName switch
            {
                var name when name.Contains("email") || name.Contains("telefone") || name.Contains("celular") => "Contato",
                var name when name.Contains("endereco") || name.Contains("cep") || name.Contains("cidade") ||
                                name.Contains("estado") || name.Contains("bairro") => "Endereço",
                var name when name.Contains("valor") || name.Contains("preco") || name.Contains("custo") => "Financeiro",
                var name when name.Contains("observa") || name.Contains("descricao") || name.Contains("nota") => "Observações",
                _ => "Dados Básicos"
            };
        }

        private string GetSectionIcon(string sectionName)
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

        private string GetDefaultTitle(string action)
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

        private string GetDefaultSubtitle(string action)
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

        #endregion

        /// <summary>
        /// Construir ViewModel para formulário com abas
        /// </summary>
        private TabbedFormViewModel BuildTabbedFormViewModel(T entity, string action)
        {
            var formConfig = typeof(T).GetCustomAttribute<FormConfigAttribute>() ?? new FormConfigAttribute();
            var formTabs = typeof(T).GetCustomAttribute<FormTabsAttribute>() ?? new FormTabsAttribute();

            var viewModel = new TabbedFormViewModel
            {
                Title = formConfig.Title ?? GetDefaultTitle(action),
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

        #region Métodos para Sistema de Abas

        /// <summary>
        /// Configurar abas da entidade
        /// </summary>
        protected virtual List<FormTabViewModel> ConfigureTabs(T entity)
        {
            var tabs = new List<FormTabViewModel>
            {
                // Aba principal sempre existe
                new FormTabViewModel
                {
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
            if (requiredRoles == null || requiredRoles.Length == 0)
            {
                return true;
            }

            return requiredRoles.Any(role => User.IsInRole(role));
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
    }
}