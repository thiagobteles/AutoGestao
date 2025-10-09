using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Helpers;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoGestao.Controllers.Base
{
    public class TabContentController(ApplicationDbContext context, ILogger<TabContentController> logger) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<TabContentController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> LoadTab(string parentController, long parentId, string tabController, string mode = "Index")
        {
            try
            {
                _logger.LogInformation("LoadTab chamado: ParentController={ParentController}, ParentId={ParentId}, TabController={TabController}, Mode={Mode}",
                    parentController, parentId, tabController, mode);

                // Validar ID
                if (parentId <= 0)
                {
                    return PartialView("_TabError", new { Message = "ID do registro pai é inválido. Salve o registro antes de acessar as abas." });
                }

                // Buscar tipo da entidade
                var assembly = Assembly.GetExecutingAssembly();
                var entityTypeName = GetEntityTypeFromController(tabController);

                var entityType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == entityTypeName && typeof(BaseEntidade).IsAssignableFrom(t));

                if (entityType == null)
                {
                    _logger.LogWarning("Entidade não encontrada: {EntityTypeName}", entityTypeName);
                    return PartialView("_TabError", new { Message = $"Entidade '{entityTypeName}' não encontrada" });
                }

                // Buscar propriedade de chave estrangeira
                var foreignKeyProperty = GetForeignKeyProperty(entityType, parentController);

                if (foreignKeyProperty == null)
                {
                    _logger.LogWarning("Propriedade FK não encontrada para {EntityType} -> {ParentController}",
                        entityType.Name, parentController);
                }

                // Construir ViewModel
                var viewModel = await BuildTabViewModel(entityType, parentId, foreignKeyProperty, mode, tabController);
                return PartialView("_TabContent", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar tab: {Message}", ex.Message);
                return PartialView("_TabError", new { Message = $"Erro: {ex.Message}" });
            }
        }

        private static string GetEntityTypeFromController(string controllerName)
        {
            // Remover sufixo "Controller" se existir
            if (controllerName.EndsWith("Controller"))
            {
                controllerName = controllerName[..^10];
            }

            // Casos especiais para Veículo
            if (controllerName.StartsWith("Veiculo"))
            {
                return controllerName;
            }

            // Remover 's' do plural se necessário
            if (controllerName.EndsWith("s") && controllerName.Length > 1)
            {
                return controllerName[..^1];
            }

            return controllerName;
        }

        private PropertyInfo? GetForeignKeyProperty(Type entityType, string parentController)
        {
            // Remover sufixo "Controller" se existir
            if (parentController.EndsWith("Controller"))
            {
                parentController = parentController[..^10];
            }

            // Remover 's' do plural
            var parentEntityName = parentController.EndsWith("s") && parentController.Length > 1
                ? parentController[..^1]
                : parentController;

            // Tentar variações do nome da FK
            var possibleNames = new[]
            {
                $"Id{parentEntityName}",
                $"{parentEntityName}Id",
                $"Id{parentController}",
                $"{parentController}Id"
            };

            foreach (var name in possibleNames)
            {
                var prop = entityType.GetProperty(name);
                if (prop != null && prop.PropertyType == typeof(long))
                {
                    _logger.LogInformation("FK encontrada: {PropertyName} para {EntityType}", name, entityType.Name);
                    return prop;
                }
            }

            return null;
        }

        private async Task<TabContentViewModel> BuildTabViewModel(Type entityType, long parentId, PropertyInfo? foreignKeyProperty, string mode, string tabController)
        {
            var method = typeof(TabContentController)
                .GetMethod(nameof(BuildTabViewModelGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(entityType);

            if (method == null)
            {
                throw new Exception("Método BuildTabViewModelGeneric não encontrado");
            }

            var result = method.Invoke(this, [parentId, foreignKeyProperty, mode, tabController]);
            return await (Task<TabContentViewModel>)result!;
        }

        private async Task<TabContentViewModel> BuildTabViewModelGeneric<TEntity>(long parentId, PropertyInfo? foreignKeyProperty, string mode, string tabController) where TEntity : BaseEntidade
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (foreignKeyProperty != null)
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, foreignKeyProperty);
                var constant = System.Linq.Expressions.Expression.Constant(parentId);
                var equals = System.Linq.Expressions.Expression.Equal(property, constant);
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

                query = query.Where(lambda);

                _logger.LogInformation("Filtro aplicado: {FKProperty} = {ParentId}", foreignKeyProperty.Name, parentId);
            }

            var items = await query.OrderByDescending(e => e.Id).ToListAsync();

            _logger.LogInformation("Registros encontrados: {Count}", items.Count);

            var gridColumns = GridColumnBuilder.BuildColumns<TEntity>();
            var entityType = typeof(TEntity);
            var formConfig = entityType.GetCustomAttribute<FormConfigAttribute>() ?? new FormConfigAttribute();

            // DETERMINAR O NOME CORRETO DO CONTROLLER
            string controllerName;

            // Se o tabController já termina com "s", usar como está
            if (tabController.EndsWith("s"))
            {
                controllerName = tabController;
            }
            // Se começa com "Veiculo", manter o padrão (ex: VeiculoDocumentos)
            else if (tabController.StartsWith("Veiculo"))
            {
                controllerName = tabController.EndsWith("s") ? tabController : tabController + "s";
            }
            // Caso padrão: adicionar "s" no final
            else
            {
                controllerName = tabController + "s";
            }

            _logger.LogInformation("Controller determinado: {ControllerName} (original: {TabController})", controllerName, tabController);

            return new TabContentViewModel
            {
                EntityType = entityType.Name,
                ControllerName = controllerName,
                Mode = mode,
                ParentId = parentId,
                Items = items.Cast<object>().ToList(),
                Columns = gridColumns.Where(c => c.Type != Enumerador.Gerais.EnumGridColumnType.Actions).ToList(),
                Title = formConfig.Title ?? entityType.Name,
                Icon = formConfig.Icon ?? "fas fa-list",
                ForeignKeyProperty = foreignKeyProperty?.Name,
                CanCreate = mode != "Details",
                CanEdit = mode != "Details",
                CanDelete = mode != "Details"
            };
        }
    }
}