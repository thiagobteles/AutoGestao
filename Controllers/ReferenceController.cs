using AutoGestao.Entidades;
using AutoGestao.Models;
using AutoGestao.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Reflection;

namespace AutoGestao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceController(GenericReferenceService referenceService, ILogger<ReferenceController> logger) : ControllerBase
    {
        private readonly GenericReferenceService _referenceService = referenceService;
        private readonly ILogger<ReferenceController> _logger = logger;

        // Cache thread-safe para mapeamento de entidades
        private static readonly ConcurrentDictionary<string, Type> _entityTypeCache = new();
        private static bool _cacheInitialized = false;
        private static readonly Lock _cacheLock = new();

        /// <summary>
        /// Busca referências usando termo de pesquisa
        /// </summary>
        [HttpPost("Search")]
        public async Task<ActionResult<List<ReferenceItem>>> Search([FromBody] ReferenceSearchRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EntityType))
                {
                    return BadRequest(new { error = "EntityType é obrigatório" });
                }

                if (string.IsNullOrEmpty(request.SearchTerm) || request.SearchTerm.Length < 2)
                {
                    return BadRequest(new { error = "SearchTerm deve ter pelo menos 2 caracteres" });
                }

                if (request.PageSize <= 0 || request.PageSize > 50)
                {
                    request.PageSize = 10;
                }

                var entityType = GetEntityType(request.EntityType);
                if (entityType == null)
                {
                    _logger.LogWarning("EntityType '{EntityType}' não encontrado", request.EntityType);
                    return BadRequest(new { error = $"EntityType '{request.EntityType}' não encontrado. Entidades disponíveis: {string.Join(", ", GetAvailableEntityTypes())}" });
                }

                _logger.LogInformation("Buscando {EntityType} com termo '{SearchTerm}'",
                    request.EntityType, request.SearchTerm);

                var results = await InvokeSearchAsync(entityType, request.SearchTerm, request.PageSize, request.Filters);

                _logger.LogInformation("Encontrados {Count} resultados", results.Count);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar {EntityType}", request.EntityType);
                return StatusCode(500, new { error = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um item específico por ID de forma genérica
        /// </summary>
        [HttpPost("GetById")]
        public async Task<ActionResult<ReferenceItem>> GetById([FromBody] ReferenceGetByIdRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EntityType))
                {
                    return BadRequest(new { error = "EntityType é obrigatório" });
                }

                if (string.IsNullOrEmpty(request.Id))
                {
                    return BadRequest(new { error = "Id é obrigatório" });
                }

                var entityType = GetEntityType(request.EntityType);
                if (entityType == null)
                {
                    _logger.LogWarning("EntityType '{EntityType}' não encontrado", request.EntityType);
                    return BadRequest(new { error = $"EntityType '{request.EntityType}' não encontrado. Entidades disponíveis: {string.Join(", ", GetAvailableEntityTypes())}" });
                }

                _logger.LogInformation("Buscando {EntityType} com ID '{Id}'", request.EntityType, request.Id);

                var result = await InvokeGetByIdAsync(entityType, request.Id);

                if (result == null)
                {
                    _logger.LogWarning("Não encontrado {EntityType} com ID '{Id}'", request.EntityType, request.Id);
                    return NotFound(new { error = "Item não encontrado" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar {EntityType} com ID '{Id}'", request.EntityType, request.Id);
                return StatusCode(500, new { error = "Erro interno do servidor", details = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para listar todas as entidades disponíveis (útil para debugging)
        /// </summary>
        [HttpGet("AvailableEntities")]
        public ActionResult<List<string>> GetAvailableEntities()
        {
            InitializeEntityCache();
            return Ok(_entityTypeCache.Keys.OrderBy(k => k).ToList());
        }

        #region Helper Methods

        /// <summary>
        /// Obtém o tipo da entidade de forma dinâmica
        /// </summary>
        private static Type? GetEntityType(string entityTypeName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeName))
            {
                return null;
            }

            InitializeEntityCache();

            // Busca case-insensitive
            var normalizedName = entityTypeName.Trim().ToLowerInvariant();

            if (_entityTypeCache.TryGetValue(normalizedName, out var type))
            {
                return type;
            }

            return null;
        }

        /// <summary>
        /// Inicializa o cache de entidades automaticamente
        /// </summary>
        private static void InitializeEntityCache()
        {
            if (_cacheInitialized)
            {
                return;
            }

            lock (_cacheLock)
            {
                if (_cacheInitialized)
                {
                    return;
                }

                try
                {
                    var assembly = Assembly.GetExecutingAssembly();

                    // Buscar todas as classes que herdam de BaseEntidade
                    var entityTypes = assembly.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && IsEntity(t))
                        .ToList();

                    foreach (var type in entityTypes)
                    {
                        var key = type.Name.ToLowerInvariant();
                        _entityTypeCache.TryAdd(key, type);

                        // Adicionar também versões sem sufixos comuns para facilitar busca
                        if (type.Name.EndsWith("Entity", StringComparison.OrdinalIgnoreCase))
                        {
                            var alternativeKey = type.Name[..^6].ToLowerInvariant(); // Remove "Entity"
                            _entityTypeCache.TryAdd(alternativeKey, type);
                        }
                    }

                    _cacheInitialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao inicializar cache de entidades: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Verifica se um tipo é uma entidade válida
        /// </summary>
        private static bool IsEntity(Type type)
        {
            // Verifica se herda de BaseEntidade ou BaseEntidadeEmpresa
            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                if (baseType == typeof(BaseEntidade))
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Obtém lista de entidades disponíveis
        /// </summary>
        private static List<string> GetAvailableEntityTypes()
        {
            InitializeEntityCache();
            return _entityTypeCache.Keys.OrderBy(k => k).Take(10).ToList();
        }

        /// <summary>
        /// Invoca GetByIdAsync usando reflection
        /// </summary>
        private async Task<ReferenceItem?> InvokeGetByIdAsync(Type entityType, string id)
        {
            var method = typeof(GenericReferenceService)
                .GetMethod(nameof(GenericReferenceService.GetByIdAsync))
                ?.MakeGenericMethod(entityType);

            if (method == null)
            {
                return null;
            }

            var task = (Task?)method.Invoke(_referenceService, new object[] { id });
            if (task == null)
            {
                return null;
            }

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task) as ReferenceItem;
        }

        /// <summary>
        /// Invoca SearchAsync usando reflection
        /// </summary>
        private async Task<List<ReferenceItem>> InvokeSearchAsync(Type entityType, string searchTerm, int pageSize, Dictionary<string, string>? filters)
        {
            var method = typeof(GenericReferenceService)
                .GetMethod(nameof(GenericReferenceService.SearchAsync))
                ?.MakeGenericMethod(entityType);

            if (method == null)
            {
                return [];
            }

            var task = (Task?)method.Invoke(_referenceService, new object?[] { searchTerm, pageSize, filters });
            if (task == null)
            {
                return [];
            }

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            return (resultProperty?.GetValue(task) as List<ReferenceItem>) ?? [];
        }

        #endregion
    }
}