using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Models;
using AutoGestao.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceController(GenericReferenceService referenceService, ILogger<ReferenceController> logger) : ControllerBase
    {
        private readonly GenericReferenceService _referenceService = referenceService;
        private readonly ILogger<ReferenceController> _logger = logger;

        // Mapeamento de EntityType para Type real
        private static readonly Dictionary<string, Type> EntityTypeMap = new()
        {
            { "cliente", typeof(Cliente) },
            { "fornecedor", typeof(Fornecedor) },
            { "vendedor", typeof(Vendedor) },
            { "veiculomarca", typeof(VeiculoMarca) },
            { "veiculomarcamodelo", typeof(VeiculoMarcaModelo) },
            { "veiculocor", typeof(VeiculoCor) }
        };

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
                    return BadRequest(new { error = $"EntityType '{request.EntityType}' não suportado" });
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
                    return BadRequest(new { error = $"EntityType '{request.EntityType}' não suportado" });
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

        #region Helper Methods

        private static Type? GetEntityType(string entityType)
        {
            return EntityTypeMap.TryGetValue(entityType.ToLower(), out var type) ? type : null;
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