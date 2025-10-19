using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Helpers;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using AutoGestao.Services;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoGestao.Controllers.Base
{
    /// <summary>
    /// Controller Ultra-Genérico com Auto-Discovery e Convention-Based Configuration
    /// Elimina 90% da duplicação de código entre controllers
    /// </summary>
    /// <typeparam name="T">Entidade que herda de BaseEntidade</typeparam>
    public abstract class AutoGridController<T> : Controller where T : BaseEntidade, new()
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IFileStorageService _fileStorageService;
        protected readonly ILogger<AutoGridController<T>>? _logger;
        protected readonly EntityMetadataCache<T> _metadata;

        protected AutoGridController(
            ApplicationDbContext context,
            IFileStorageService fileStorageService,
            ILogger<AutoGridController<T>>? logger = null)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _logger = logger;
            _metadata = EntityMetadataCache<T>.Instance;
        }

        #region Auto-Discovery Grid Configuration

        /// <summary>
        /// Configuração automática da grid baseada em convenções e atributos
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 20,
            string? orderBy = null,
            string orderDirection = "asc")
        {
            try
            {
                var gridConfig = await BuildAutoGridConfigurationAsync();
                var query = BuildAutoFilteredQuery();

                // Auto-pagination
                var totalRecords = await query.CountAsync();
                var items = await query
                    .ApplyAutoOrdering(orderBy, orderDirection, _metadata)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new StandardGridViewModel<T>
                {
                    Items = items,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    OrderBy = orderBy ?? _metadata.DefaultOrderField,
                    OrderDirection = orderDirection,
                    Filters = gridConfig.Filters,
                    Columns = gridConfig.Columns,
                    RowActions = gridConfig.RowActions,
                    Title = gridConfig.Title,
                    SubTitle = gridConfig.Subtitle,
                    Icon = gridConfig.Icon,
                    CreateUrl = GetConventionBasedUrl("Create"),
                    EntityName = typeof(T).Name
                };

                // Auto-apply custom configurations se existirem
                ConfigureCustomGrid(viewModel);

                return View("_AutoGrid", viewModel);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao carregar grid para {EntityType}", typeof(T).Name);
                throw;
            }
        }

        /// <summary>
        /// Constrói automaticamente a configuração da grid
        /// </summary>
        protected virtual async Task<AutoGridConfiguration> BuildAutoGridConfigurationAsync()
        {
            var config = new AutoGridConfiguration();

            // 1. Auto-discover título e configurações básicas
            var formConfig = _metadata.FormConfig;
            config.Title = formConfig?.Title ?? GetConventionBasedTitle();
            config.Subtitle = formConfig?.Subtitle ?? GetConventionBasedSubtitle();
            config.Icon = formConfig?.Icon ?? GetConventionBasedIcon();

            // 2. Auto-build filtros
            config.Filters = await BuildAutoFiltersAsync();

            // 3. Auto-build colunas
            config.Columns = BuildAutoColumnsAsync();

            // 4. Auto-build ações
            config.RowActions = await BuildAutoRowActionsAsync();

            return config;
        }

        /// <summary>
        /// Constrói filtros automaticamente baseado nos campos da entidade
        /// </summary>
        protected virtual async Task<List<GridFilter>> BuildAutoFiltersAsync()
        {
            var filters = new List<GridFilter>
            {
                // Filtro de busca geral sempre presente
                new GridFilter
                {
                    Name = "search",
                    DisplayName = "Busca Geral",
                    Type = EnumGridFilterType.Text,
                    Placeholder = GetSearchPlaceholder()
                }
            };

            // Auto-detect enum fields para filtros
            var enumProperties = _metadata.EnumProperties;
            foreach (var prop in enumProperties)
            {
                var enumType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var displayName = prop.GetDisplayName() ?? prop.Name;

                filters.Add(new GridFilter
                {
                    Name = prop.Name.ToLowerInvariant(),
                    DisplayName = displayName,
                    Type = EnumGridFilterType.Select,
                    Placeholder = $"Selecionar {displayName.ToLower()}...",
                    Options = EnumAutomationHelper.GetEnumSelectListItems(prop.Name, typeof(T), true)
                });
            }

            // Auto-detect date fields
            var dateProperties = _metadata.DateProperties;
            foreach (var prop in dateProperties)
            {
                var displayName = prop.GetDisplayName() ?? prop.Name;
                filters.Add(new GridFilter
                {
                    Name = prop.Name.ToLowerInvariant(),
                    DisplayName = displayName,
                    Type = EnumGridFilterType.DateRange,
                    Placeholder = $"Filtrar por {displayName.ToLower()}"
                });
            }

            // Auto-detect reference fields para filtros
            var referenceProperties = _metadata.ReferenceProperties;
            foreach (var prop in referenceProperties.Take(3)) // Limitar a 3 para não sobrecarregar
            {
                var displayName = prop.GetDisplayName() ?? prop.Name;
                filters.Add(new GridFilter
                {
                    Name = prop.Name.ToLowerInvariant(),
                    DisplayName = displayName,
                    Type = EnumGridFilterType.Reference,
                    Placeholder = $"Selecionar {displayName.ToLower()}...",
                    ReferenceType = prop.GetFormFieldAttribute()?.Reference
                });
            }

            return filters;
        }

        /// <summary>
        /// Constrói colunas automaticamente
        /// </summary>
        protected virtual List<GridColumn> BuildAutoColumnsAsync()
        {
            var columns = new List<GridColumn>();

            // Adicionar colunas baseadas em atributos Grid
            var gridProperties = _metadata.GridProperties;

            foreach (var prop in gridProperties.OrderBy(p => p.GetGridOrder()))
            {
                var atributo = prop.GetGridAttribute();
                if (atributo == null || atributo is not GridColumn)
                {
                    continue;
                }

                var gridAttr = atributo as GridColumn;

                columns.Add(new GridColumn
                {
                    PropertyName = prop.Name,
                    DisplayName = gridAttr.DisplayName ?? prop.GetDisplayName() ?? prop.Name,
                    Width = gridAttr.Width,
                    Format = gridAttr.Format,
                    Sortable = gridAttr.Sortable,
                    ShowInGrid = gridAttr.ShowInGrid,
                    Order = gridAttr.Order,
                    CssClass = gridAttr.CssClass,
                    Template = gridAttr.Template
                });
            }

            // Se não há colunas definidas, usar convenções
            if (!columns.Any())
            {
                columns = BuildConventionBasedColumns();
            }

            return columns;
        }

        /// <summary>
        /// Constrói ações automaticamente baseado em convenções
        /// </summary>
        protected virtual async Task<List<GridRowAction>> BuildAutoRowActionsAsync()
        {
            var actions = new List<GridRowAction>();

            // Ações padrão baseadas em convenções
            actions.AddRange(GetStandardRowActions());

            // Auto-detect ações específicas baseado em controllers relacionados
            var relatedActions = await DiscoverRelatedActionsAsync();
            actions.AddRange(relatedActions);

            return actions;
        }

        #endregion

        #region Convention-Based Helpers

        /// <summary>
        /// Obtém título baseado em convenção
        /// </summary>
        protected virtual string GetConventionBasedTitle()
        {
            var entityName = typeof(T).Name;
            return entityName; // Manter SINGULAR conforme convenção do projeto
        }

        /// <summary>
        /// Obtém subtitle baseado em convenção
        /// </summary>
        protected virtual string GetConventionBasedSubtitle()
        {
            var entityName = typeof(T).Name;
            return $"Gerencie os registros de {entityName.ToLower()}";
        }

        /// <summary>
        /// Obtém ícone baseado em convenção
        /// </summary>
        protected virtual string GetConventionBasedIcon()
        {
            var entityName = typeof(T).Name.ToLower();

            // Mapeamento inteligente de ícones
            return entityName switch
            {
                var name when name.Contains("veiculo") => "fas fa-car",
                var name when name.Contains("cliente") => "fas fa-users",
                var name when name.Contains("fornecedor") => "fas fa-truck",
                var name when name.Contains("vendedor") => "fas fa-user-tie",
                var name when name.Contains("produto") => "fas fa-box",
                var name when name.Contains("servico") => "fas fa-tools",
                var name when name.Contains("financeiro") => "fas fa-dollar-sign",
                var name when name.Contains("documento") => "fas fa-file",
                var name when name.Contains("relatorio") => "fas fa-chart-bar",
                _ => "fas fa-database"
            };
        }

        /// <summary>
        /// Gera URL baseado em convenção
        /// </summary>
        protected virtual string GetConventionBasedUrl(string action, object? parameters = null)
        {
            var controllerName = GetControllerName();
            var url = $"/{controllerName}/{action}";

            if (parameters != null)
            {
                var props = parameters.GetType().GetProperties();
                var queryParams = props.Select(p => $"{p.Name}={{{p.Name}}}");
                url += "?" + string.Join("&", queryParams);
            }

            return url;
        }

        /// <summary>
        /// Obtém placeholder para busca baseado nos campos searchable
        /// </summary>
        protected virtual string GetSearchPlaceholder()
        {
            var searchableFields = _metadata.SearchableProperties
                .Take(4)
                .Select(p => p.GetDisplayName() ?? p.Name);

            return searchableFields.Any()
                ? string.Join(", ", searchableFields) + "..."
                : "Buscar...";
        }

        /// <summary>
        /// Constrói colunas baseado em convenções quando não há atributos
        /// </summary>
        protected virtual List<GridColumn> BuildConventionBasedColumns()
        {
            var columns = new List<GridColumn>();
            var properties = typeof(T).GetProperties()
                .Where(p => AutoGridController<T>.ShouldIncludeInGrid(p))
                .Take(6) // Máximo 6 colunas por convenção
                .ToList();

            foreach (var prop in properties)
            {
                columns.Add(new GridColumn
                {
                    PropertyName = prop.Name,
                    DisplayName = prop.GetDisplayName() ?? prop.Name,
                    Sortable = AutoGridController<T>.IsPropertySortable(prop),
                    ShowInGrid = true,
                    Order = AutoGridController<T>.GetPropertyOrder(prop)
                });
            }

            return columns;
        }

        /// <summary>
        /// Ações padrão para todas as entidades
        /// </summary>
        protected virtual List<GridRowAction> GetStandardRowActions()
        {
            var controllerName = GetControllerName();

            return
            [
                new()
                {
                    Name = "View",
                    DisplayName = "Visualizar",
                    Icon = "fas fa-eye",
                    Url = $"/{controllerName}/Details/{{id}}",
                    CssClass = "btn-outline-info"
                },
                new()
                {
                    Name = "Edit",
                    DisplayName = "Editar",
                    Icon = "fas fa-edit",
                    Url = $"/{controllerName}/Edit/{{id}}",
                    CssClass = "btn-outline-primary"
                },
                new()
                {
                    Name = "Delete",
                    DisplayName = "Excluir",
                    Icon = "fas fa-trash",
                    Url = $"/{controllerName}/Delete/{{id}}",
                    CssClass = "btn-outline-danger",
                    RequireConfirmation = true,
                    ConfirmationMessage = "Deseja realmente excluir este registro?"
                }
            ];
        }

        /// <summary>
        /// Descobre ações relacionadas automaticamente
        /// </summary>
        protected virtual async Task<List<GridRowAction>> DiscoverRelatedActionsAsync()
        {
            var actions = new List<GridRowAction>();
            var entityName = typeof(T).Name;

            // Auto-discover baseado em controllers existentes
            var controllerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Controller)) &&
                           t.Name.Contains(entityName))
                .ToList();

            foreach (var controllerType in controllerTypes)
            {
                var relatedActions = await AutoGridController<T>.DiscoverActionsForController(controllerType);
                actions.AddRange(relatedActions);
            }

            return actions;
        }

        #endregion

        #region Query Building

        /// <summary>
        /// Constrói query com filtros automáticos
        /// </summary>
        protected virtual IQueryable<T> BuildAutoFilteredQuery()
        {
            var query = GetBaseQuery();
            var filters = ExtractFiltersFromRequest();

            // Auto-apply search filter
            if (filters.TryGetValue("search", out var searchValue) && !string.IsNullOrEmpty(searchValue.ToString()))
            {
                query = ApplyAutoSearchFilter(query, searchValue.ToString()!);
            }

            // Auto-apply other filters
            query = ApplyAutoFilters(query, filters);

            return query;
        }

        /// <summary>
        /// Aplica filtro de busca automático nos campos searchable
        /// </summary>
        protected virtual IQueryable<T> ApplyAutoSearchFilter(IQueryable<T> query, string searchTerm)
        {
            var searchableProperties = _metadata.SearchableProperties;
            if (!searchableProperties.Any())
            {
                return query;
            }

            // Construir expressão OR dinâmica
            return query.Where(entity =>
                searchableProperties.Any(prop =>
                    EF.Property<string>(entity, prop.Name).Contains(searchTerm)));
        }

        /// <summary>
        /// Aplica filtros automáticos
        /// </summary>
        protected virtual IQueryable<T> ApplyAutoFilters(IQueryable<T> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "search")
                {
                    continue; // Já tratado
                }

                var property = _metadata.GetPropertyByName(filter.Key);
                if (property == null)
                {
                    continue;
                }

                query = AutoGridController<T>.ApplyFilterToProperty(query, property, filter.Value);
            }

            return query;
        }

        /// <summary>
        /// Query base com includes automáticos
        /// </summary>
        protected virtual IQueryable<T> GetBaseQuery()
        {
            var query = _context.Set<T>().AsQueryable();

            try
            {
                // Auto-include navigation properties que realmente existem no EF
                var entityType = _context.Model.FindEntityType(typeof(T));

                if (entityType != null)
                {
                    var navigationProperties = _metadata.NavigationProperties;

                    foreach (var navProp in navigationProperties)
                    {
                        try
                        {
                            // Verificar se a navegação existe no modelo EF
                            var navigation = entityType.FindNavigation(navProp.Name);
                            if (navigation != null)
                            {
                                query = query.Include(navProp.Name);
                            }
                            else
                            {
                                _logger?.LogDebug("Navegação {NavigationName} não encontrada no modelo EF para {EntityType}", navProp.Name, typeof(T).Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, "Erro ao incluir navegação {NavigationName} para {EntityType}", navProp.Name, typeof(T).Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao configurar includes automáticos para {EntityType}", typeof(T).Name);
                // Se houver erro, continuar sem includes
            }

            // Filtro automático por empresa
            query = query.Where(e => e.IdEmpresa == GetCurrentEmpresaId());

            return query;
        }

        #endregion

        #region Navigation Validation Helper

        /// <summary>
        /// Valida se uma navegação existe no modelo EF
        /// </summary>
        protected virtual bool IsValidNavigation(string navigationName)
        {
            try
            {
                var entityType = _context.Model.FindEntityType(typeof(T));
                return entityType?.FindNavigation(navigationName) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtém apenas navegações válidas para Include
        /// </summary>
        protected virtual List<string> GetValidNavigationNames()
        {
            try
            {
                var entityType = _context.Model.FindEntityType(typeof(T));
                if (entityType == null)
                {
                    return [];
                }

                return entityType.GetNavigations()
                    .Select(n => n.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao obter navegações válidas para {EntityType}", typeof(T).Name);
                return [];
            }
        }

        #endregion

        #region Abstract/Virtual Methods para Override

        /// <summary>
        /// Permite override da configuração automática
        /// </summary>
        protected virtual StandardGridViewModel<T> ConfigureCustomGrid(StandardGridViewModel<T> gridViewModel)
        {
            return gridViewModel; // Override em controllers específicos se necessário
        }

        /// <summary>
        /// Permite override da aplicação de filtros
        /// </summary>
        protected virtual IQueryable<T> ApplyCustomFilters(IQueryable<T> query, Dictionary<string, object> filters)
        {
            return query; // Override em controllers específicos se necessário
        }

        /// <summary>
        /// Permite adicionar ações customizadas
        /// </summary>
        protected virtual List<GridRowAction> GetCustomRowActions()
        {
            return []; // Override em controllers específicos
        }

        #endregion

        #region Utility Methods

        protected virtual string GetControllerName()
        {
            return ControllerContext.ActionDescriptor.ControllerName;
        }

        protected virtual long GetCurrentEmpresaId()
        {
            // Implementar lógica para obter empresa do usuário atual
            return 1; // Placeholder
        }

        protected virtual Dictionary<string, object> ExtractFiltersFromRequest()
        {
            var filters = new Dictionary<string, object>();

            foreach (var key in Request.Query.Keys)
            {
                var value = Request.Query[key].ToString();
                if (!string.IsNullOrEmpty(value) &&
                    !new[] { "page", "pageSize", "orderBy", "orderDirection" }.Contains(key))
                {
                    filters[key] = value;
                }
            }

            return filters;
        }

        private static bool ShouldIncludeInGrid(PropertyInfo property)
        {
            // Convenções para incluir propriedades na grid
            var excludedTypes = new[] { typeof(DateTime), typeof(byte[]) };
            var excludedNames = new[] { "Id", "IdEmpresa", "DataCadastro", "DataAlteracao" };

            return !excludedTypes.Contains(property.PropertyType) &&
                   !excludedNames.Contains(property.Name) &&
                   property.CanRead &&
                   !property.PropertyType.IsClass ||
                   property.PropertyType == typeof(string);
        }

        private static bool IsPropertySortable(PropertyInfo property)
        {
            var nonsortableTypes = new[] { typeof(byte[]), typeof(object) };
            return !nonsortableTypes.Contains(property.PropertyType);
        }

        private static int GetPropertyOrder(PropertyInfo property)
        {
            // Ordem por convenção
            return property.Name switch
            {
                "Codigo" => 1,
                "Nome" => 2,
                "Descricao" => 3,
                "DataCadastro" => 999,
                _ => 500
            };
        }

        private static async Task<List<GridRowAction>> DiscoverActionsForController(Type controllerType)
        {
            // Implementar discovery de ações baseado no controller
            return [];
        }

        private static IQueryable<T> ApplyFilterToProperty(IQueryable<T> query, PropertyInfo property, object value)
        {
            // Implementar aplicação de filtro baseado no tipo da propriedade
            return query;
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Configuração automática da grid
    /// </summary>
    public class AutoGridConfiguration
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Icon { get; set; } = "";
        public List<GridFilter> Filters { get; set; } = [];
        public List<GridColumn> Columns { get; set; } = [];
        public List<GridRowAction> RowActions { get; set; } = [];
    }

    #endregion
}