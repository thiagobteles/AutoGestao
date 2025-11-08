using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models;
using AutoGestao.Models.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoGestao.Controllers.Base
{
    [Authorize]
    [Route("[controller]")]
    public class TabContentController(ApplicationDbContext context, ILogger<TabContentController> logger) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<TabContentController> _logger = logger;

        [HttpGet("LoadTab")]
        public async Task<IActionResult> LoadTab(
            [FromQuery] string controller,
            [FromQuery] string tab,
            [FromQuery] long parentId,
            [FromQuery] string parentController,
            [FromQuery] string? mode = null)
        {
            try
            {
                _logger.LogInformation("LoadTab chamado - Controller: {Controller}, Tab: {Tab}, ParentId: {ParentId}, ParentController: {ParentController}",
                    controller, tab, parentId, parentController);

                if (string.IsNullOrEmpty(controller))
                {
                    return BadRequest("Parâmetro 'controller' é obrigatório");
                }

                if (string.IsNullOrEmpty(parentController))
                {
                    return BadRequest("Parâmetro 'parentController' é obrigatório");
                }

                var entityType = GetEntityType(controller);
                if (entityType == null)
                {
                    _logger.LogError("Entity type não encontrado para controller: {Controller}", controller);
                    return BadRequest($"Controller {controller} não encontrado");
                }

                var filterField = GetParentFieldName(controller, parentController);
                if (string.IsNullOrEmpty(filterField))
                {
                    _logger.LogError("Campo de filtro não encontrado para {Controller} com pai {ParentController}",
                        controller, parentController);
                    return BadRequest($"Não foi possível determinar o campo de filtro");
                }

                _logger.LogInformation("EntityType: {EntityType}, FilterField: {FilterField}",
                    entityType.Name, filterField);

                var filterProperty = entityType.GetProperty(filterField);
                if (filterProperty == null)
                {
                    _logger.LogError("Propriedade {FilterField} não encontrada em {EntityType}",
                        filterField, entityType.Name);
                    return BadRequest($"Campo {filterField} não encontrado na entidade {entityType.Name}");
                }

                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                var genericSetMethod = setMethod.MakeGenericMethod(entityType);
                var dbSet = genericSetMethod.Invoke(_context, null);

                var asQueryableMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "AsQueryable" && m.GetParameters().Length == 1)
                    .MakeGenericMethod(entityType);

                var query = asQueryableMethod.Invoke(null, [dbSet]) as IQueryable;

                _logger.LogInformation("Query inicial criada");

                // Entidades que precisam ignorar TODOS os query filters (incluindo dos relacionamentos)
                var entitiesToIgnoreQueryFilters = new[]
                {
                    "UsuarioEmpresaCliente" // Tabela N:N - precisa ignorar filtros para ver todas as empresas vinculadas
                };

                if (entitiesToIgnoreQueryFilters.Contains(entityType.Name))
                {
                    _logger.LogInformation("Aplicando IgnoreQueryFilters para {EntityType}", entityType.Name);
                    var ignoreQueryFiltersMethod = typeof(EntityFrameworkQueryableExtensions)
                        .GetMethod("IgnoreQueryFilters")
                        .MakeGenericMethod(entityType);
                    query = ignoreQueryFiltersMethod.Invoke(null, [query]) as IQueryable;
                }

                query = ApplyParentFilter(query, entityType, filterField, parentId);
                _logger.LogInformation("Filtro por parent aplicado: {FilterField} = {ParentId}", filterField, parentId);

                // Entidades que NÃO devem ter filtro de tenant (são compartilhadas ou N:N)
                var entitiesToExcludeFromTenantFilter = new[]
                {
                    "Usuario",
                    "Empresa",
                    "UsuarioEmpresaCliente" // Tabela N:N - deve mostrar todas as empresas do usuário
                };

                var idEmpresaProperty = entityType.GetProperty("IdEmpresa");
                if (idEmpresaProperty != null && !entitiesToExcludeFromTenantFilter.Contains(entityType.Name))
                {
                    var idEmpresa = GetCurrentEmpresaId();
                    _logger.LogInformation("Aplicando filtro de empresa: IdEmpresa = {IdEmpresa}", idEmpresa);
                    query = ApplyEmpresaFilter(query, entityType, idEmpresa);
                }
                else
                {
                    _logger.LogInformation("Entidade {EntityType} não terá filtro de empresa aplicado", entityType.Name);
                }

                query = ApplyIncludes(query, entityType);
                _logger.LogInformation("Includes aplicados");

                var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .First(m => m.Name == "ToListAsync" &&
                                m.GetParameters().Length == 2 &&
                                m.GetParameters()[0].ParameterType.IsGenericType)
                    .MakeGenericMethod(entityType);

                // DEBUG: Verificar query SQL
                _logger.LogInformation("Query antes de executar: {Query}", query.ToString());

                var itemsTask = toListAsyncMethod.Invoke(null, [query, CancellationToken.None]) as Task;
                await itemsTask;

                var resultProperty = itemsTask.GetType().GetProperty("Result");
                var items = resultProperty.GetValue(itemsTask);
                var itemsList = (items as IEnumerable<object>).ToList();

                _logger.LogInformation("Itens encontrados: {Count}", itemsList.Count);

                // DEBUG: Logar dados dos itens
                foreach (var item in itemsList)
                {
                    _logger.LogInformation("Item: {Item}", item);
                }

                var gridColumns = GetGridColumns(entityType);
                var tabColumns = ConvertToTabColumns(gridColumns);

                var formConfig = entityType.GetCustomAttribute<FormConfigAttribute>();
                var title = formConfig?.Title ?? controller;
                var icon = formConfig?.Icon ?? "fas fa-list";

                _logger.LogInformation("Controller name ajustado: {Original} -> {Adjusted}", controller, controller);

                // Se o modo for "Details", não permitir criar, editar ou excluir
                var isReadOnly = mode?.Equals("Details", StringComparison.OrdinalIgnoreCase) == true;
                _logger.LogInformation("Modo: {Mode}, IsReadOnly: {IsReadOnly}", mode, isReadOnly);

                var viewModel = new TabContentViewModel
                {
                    TabId = tab,
                    Title = title,
                    Icon = icon,
                    ControllerName = controller,
                    ParentId = parentId,
                    ParentController = parentController,
                    Items = itemsList,
                    Columns = tabColumns,
                    CanCreate = !isReadOnly,
                    CanEdit = !isReadOnly,
                    CanDelete = !isReadOnly
                };

                return PartialView("_TabContent", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar tab {Tab} do controller {Controller}", tab, controller);

                return PartialView("_TabError", new
                {
                    Message = ex.Message,
                    Controller = controller,
                    Tab = tab
                });
            }
        }

        private IQueryable ApplyIncludes(IQueryable? query, Type entityType)
        {
            // Tratamento especial para entidades específicas que precisam de Includes explícitos
            if (entityType.Name == "UsuarioEmpresaCliente")
            {
                _logger.LogInformation("Aplicando Includes explícitos para UsuarioEmpresaCliente");
                var includeMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .First(m => m.Name == "Include" &&
                                m.GetParameters().Length == 2 &&
                                m.GetParameters()[1].ParameterType == typeof(string))
                    .MakeGenericMethod(entityType);

                query = includeMethod.Invoke(null, [query, "EmpresaCliente"]) as IQueryable;
                _logger.LogInformation("Include aplicado para EmpresaCliente");
                query = includeMethod.Invoke(null, [query, "Usuario"]) as IQueryable;
                _logger.LogInformation("Include aplicado para Usuario");

                return query;
            }

            var properties = entityType.GetProperties()
                .Where(p => p.PropertyType.IsClass &&
                            p.PropertyType != typeof(string) &&
                            !p.PropertyType.IsArray &&
                            !p.PropertyType.IsGenericType &&
                            p.CanRead &&
                            p.GetGetMethod() != null &&
                            p.GetGetMethod().IsPublic &&
                            !p.GetGetMethod().IsStatic);

            foreach (var property in properties)
            {
                try
                {
                    // Verificar se a propriedade tem o atributo [ForeignKey] ou se o nome começa com Id
                    var propertyName = property.Name;

                    // Pular propriedades de auditoria que não são navegações reais
                    var skipProperties = new[] {
                        "CriadoPorUsuario",
                        "AlteradoPorUsuario",
                        "ExcluidoPorUsuario"
                    };

                    if (skipProperties.Contains(propertyName))
                    {
                        _logger.LogDebug("Pulando Include de propriedade não navegável: {PropertyName}", propertyName);
                        continue;
                    }

                    // Apenas incluir propriedades que têm um campo Id correspondente na entidade
                    // Exemplo: Se existe "Veiculo", deve existir "IdVeiculo"
                    var idPropertyName = $"Id{propertyName}";
                    var idProperty = entityType.GetProperty(idPropertyName);

                    if (idProperty == null)
                    {
                        _logger.LogDebug("Pulando Include - não encontrado campo FK: {IdPropertyName}", idPropertyName);
                        continue;
                    }

                    _logger.LogDebug("Aplicando Include: {PropertyName}", propertyName);

                    var includeMethod = typeof(EntityFrameworkQueryableExtensions)
                        .GetMethods()
                        .First(m => m.Name == "Include" &&
                                    m.GetParameters().Length == 2 &&
                                    m.GetParameters()[1].ParameterType == typeof(string))
                        .MakeGenericMethod(entityType);

                    query = includeMethod.Invoke(null, [query, propertyName]) as IQueryable;
                }
                catch (Exception ex)
                {
                    // Log mas não quebra - apenas não inclui essa navegação
                    _logger.LogWarning(ex, "Erro ao aplicar Include para propriedade {PropertyName} em {EntityType}", property.Name, entityType.Name);
                }
            }

            return query;
        }

        private long GetCurrentEmpresaId()
        {
            var idEmpresaClaim = User.Claims.FirstOrDefault(c => c.Type == "IdEmpresa");
            return idEmpresaClaim != null && long.TryParse(idEmpresaClaim.Value, out var idEmpresa) 
                ? idEmpresa
                : 1;
        }

        private static IQueryable ApplyParentFilter(IQueryable query, Type entityType, string filterField, long parentId)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "x");
            var property = System.Linq.Expressions.Expression.Property(parameter, filterField);
            var constant = System.Linq.Expressions.Expression.Constant(parentId);
            var equal = System.Linq.Expressions.Expression.Equal(property, constant);
            var lambda = System.Linq.Expressions.Expression.Lambda(equal, parameter);

            var whereMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            return whereMethod.Invoke(null, [query, lambda]) as IQueryable;
        }

        private static IQueryable ApplyEmpresaFilter(IQueryable query, Type entityType, long idEmpresa)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "x");
            var property = System.Linq.Expressions.Expression.Property(parameter, "IdEmpresa");
            var constant = System.Linq.Expressions.Expression.Constant(idEmpresa);
            var equal = System.Linq.Expressions.Expression.Equal(property, constant);
            var lambda = System.Linq.Expressions.Expression.Lambda(equal, parameter);

            var whereMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            return whereMethod.Invoke(null, [query, lambda]) as IQueryable;
        }

        private static List<TabColumnDefinition> ConvertToTabColumns(List<GridColumn> gridColumns)
        {
            var tabColumns = new List<TabColumnDefinition>();

            foreach (var gridColumn in gridColumns.Where(c => c.Type != EnumGridColumnType.Actions))
            {
                tabColumns.Add(new TabColumnDefinition
                {
                    PropertyName = gridColumn.PropertyName,
                    DisplayName = gridColumn.DisplayName,
                    Width = gridColumn.Width,
                    Format = gridColumn.Format,
                    Order = gridColumn.Order,
                    NavigationPaths = gridColumn.NavigationPaths,
                    Template = gridColumn.Template,
                    IsHtmlContent = gridColumn.IsHtmlContent
                });
            }

            return tabColumns;
        }

        private static Type GetEntityType(string controller)
        {
            var entityName = controller;
            var mapping = new Dictionary<string, string>
            {
                { "Despesa", "AutoGestao.Entidades.Despesa, AutoGestao" },
                { "VeiculoDocumento", "AutoGestao.Entidades.Veiculos.VeiculoDocumento, AutoGestao" },
                { "VeiculoFoto", "AutoGestao.Entidades.Veiculos.VeiculoFoto, AutoGestao" },
                { "VeiculoNFE", "AutoGestao.Entidades.Veiculos.VeiculoNFE, AutoGestao" },
                { "VeiculoLancamento", "AutoGestao.Entidades.Veiculos.VeiculoLancamento, AutoGestao" },
            };

            if (mapping.TryGetValue(entityName, out var value))
            {
                var type = Type.GetType(value);
                if (type != null)
                {
                    return type;
                }
            }

            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes()
                .FirstOrDefault(t => t.Name == entityName &&
                                   t.Namespace != null &&
                                   t.Namespace.StartsWith("AutoGestao.Entidades"));
        }

        private static string GetParentFieldName(string controller, string parentController)
        {
            return string.IsNullOrEmpty(parentController) 
                ? null
                : $"Id{parentController}";
        }

        private static List<GridColumn> GetGridColumns(Type entityType)
        {
            var columns = new List<GridColumn>();
            var properties = entityType.GetProperties();

            foreach (var property in properties)
            {
                var gridIdAttr = property.GetCustomAttribute<GridIdAttribute>();
                if (gridIdAttr != null)
                {
                    columns.Add(new GridColumn
                    {
                        PropertyName = property.Name,
                        DisplayName = "Código",
                        Type = EnumGridColumnType.Text,
                        Width = "80px",
                        Order = 0
                    });
                    continue;
                }

                var gridMainAttr = property.GetCustomAttribute<GridMainAttribute>();
                if (gridMainAttr != null)
                {
                    columns.Add(new GridColumn
                    {
                        PropertyName = property.Name,
                        DisplayName = gridMainAttr.DisplayName,
                        Type = EnumGridColumnType.Text,
                        Width = gridMainAttr.Width ?? "auto",
                        Order = gridMainAttr.Order
                    });
                    continue;
                }

                // Processar GridComposite ANTES de GridField (GridComposite herda de GridField)
                var gridCompositeAttr = property.GetCustomAttribute<GridCompositeAttribute>();
                if (gridCompositeAttr != null)
                {
                    columns.Add(new GridColumn
                    {
                        PropertyName = property.Name,
                        DisplayName = gridCompositeAttr.DisplayName,
                        Type = EnumGridColumnType.Text,
                        Width = gridCompositeAttr.Width ?? "auto",
                        Format = gridCompositeAttr.Format,
                        Order = gridCompositeAttr.Order,
                        NavigationPaths = gridCompositeAttr.NavigationPaths,
                        Template = gridCompositeAttr.Template,
                        IsHtmlContent = !string.IsNullOrEmpty(gridCompositeAttr.Template)
                    });
                    continue;
                }

                var gridFieldAttr = property.GetCustomAttribute<GridFieldAttribute>();
                if (gridFieldAttr != null)
                {
                    columns.Add(new GridColumn
                    {
                        PropertyName = property.Name,
                        DisplayName = gridFieldAttr.DisplayName,
                        Type = EnumGridColumnType.Text,
                        Width = gridFieldAttr.Width ?? "auto",
                        Format = gridFieldAttr.Format,
                        Order = gridFieldAttr.Order
                    });
                    continue;
                }
            }

            return [.. columns.OrderBy(c => c.Order)];
        }
    }
}