using AutoGestao.Controllers;
using AutoGestao.Data;
using AutoGestao.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Controllers
{
    /// <summary>
    /// API Controller para busca e obtenção de dados de referência
    /// Suporta busca dinâmica para campos de referência do sistema
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceController(ApplicationDbContext context, ILogger<ReferenceController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ReferenceController> _logger = logger;

        /// <summary>
        /// Busca registros por termo de pesquisa
        /// </summary>
        /// <param name="request">Parâmetros da busca</param>
        /// <returns>Lista de itens encontrados</returns>
        [HttpPost("Search")]
        public async Task<ActionResult<List<ReferenceItem>>> Search([FromBody] ReferenceSearchRequest request)
        {
            try
            {
                // Validação de entrada
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
                    request.PageSize = 10; // Default
                }

                _logger.LogInformation("Buscando {EntityType} com termo '{SearchTerm}'",
                    request.EntityType, request.SearchTerm);

                var results = request.EntityType.ToLower() switch
                {
                    "cliente" => await SearchClientes(request.SearchTerm, request.PageSize),
                    "fornecedor" => await SearchFornecedores(request.SearchTerm, request.PageSize),
                    "vendedor" => await SearchVendedores(request.SearchTerm, request.PageSize),
                    "veiculomarca" => await SearchVeiculoMarcas(request.SearchTerm, request.PageSize),
                    "veiculomarcamodelo" => await SearchVeiculoMarcaModelos(request.SearchTerm, request.PageSize),
                    "veiculocor" => await SearchVeiculoCores(request.SearchTerm, request.PageSize),
                    _ => []
                };

                _logger.LogInformation("Encontrados {Count} resultados para {EntityType}", results.Count, request.EntityType);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar {EntityType} com termo '{SearchTerm}'", request.EntityType, request.SearchTerm);

                return StatusCode(500, new
                {
                    error = "Erro interno do servidor",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtém um registro específico por ID
        /// </summary>
        /// <param name="request">Parâmetros da busca por ID</param>
        /// <returns>Item encontrado ou NotFound</returns>
        [HttpPost("GetById")]
        public async Task<ActionResult<ReferenceItem>> GetById([FromBody] ReferenceGetByIdRequest request)
        {
            try
            {
                // Validação de entrada
                if (string.IsNullOrEmpty(request.EntityType))
                {
                    return BadRequest(new { error = "EntityType é obrigatório" });
                }

                if (string.IsNullOrEmpty(request.Id))
                {
                    return BadRequest(new { error = "Id é obrigatório" });
                }

                _logger.LogInformation("Buscando {EntityType} com ID '{Id}'", request.EntityType, request.Id);
                var result = request.EntityType.ToLower() switch
                {
                    "cliente" => await GetClienteById(request.Id),
                    "fornecedor" => await GetFornecedorById(request.Id),
                    "vendedor" => await GetVendedorById(request.Id),
                    "veiculomarca" => await GetVeiculoMarcaById(request.Id),
                    "veiculomarcamodelo" => await GetVeiculoMarcaModeloById(request.Id),
                    "veiculocor" => await GetVeiculoCorById(request.Id),
                    _ => null
                };

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
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor",
                    details = ex.Message
                });
            }
        }

        #region Métodos de Busca por Entidade

        /// <summary>
        /// Busca clientes por nome, CPF ou email
        /// </summary>
        private async Task<List<ReferenceItem>> SearchClientes(string term, int pageSize)
        {
            var termLower = term.ToLower();

            return await _context.Clientes
                .Where(c => c.Ativo &&
                    (
                        c.Nome.ToLower().Contains(termLower) ||
                        c.CPF != null && c.CPF.Contains(term) ||
                        c.Email != null && c.Email.ToLower().Contains(termLower)
                    ))
                .OrderBy(c => c.Nome)
                .Take(pageSize)
                .Select(c => new ReferenceItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Nome,
                        Subtitle = FormatClienteSubtitle(c.CPF, c.Email, c.Telefone)
                    })
                .ToListAsync();
        }

        /// <summary>
        /// Busca fornecedores por nome, CNPJ ou email
        /// </summary>
        private async Task<List<ReferenceItem>> SearchFornecedores(string term, int pageSize)
        {
            var termLower = term.ToLower();

            return await _context.Fornecedores
                .Where(f => f.Ativo &&
                    (
                        f.Nome.ToLower().Contains(termLower) ||
                        f.CNPJ != null && f.CNPJ.Contains(term) ||
                        f.Email != null && f.Email.ToLower().Contains(termLower)
                    ))
                .OrderBy(f => f.Nome)
                .Take(pageSize)
                .Select(f => new ReferenceItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.Nome,
                        Subtitle = FormatFornecedorSubtitle(f.CNPJ, f.Email)
                    })
                .ToListAsync();
        }

        /// <summary>
        /// Busca vendedores por nome, CPF ou email
        /// </summary>
        private async Task<List<ReferenceItem>> SearchVendedores(string term, int pageSize)
        {
            var termLower = term.ToLower();

            return await _context.Vendedores
                .Where(v => v.Ativo && 
                    (
                        v.Nome.ToLower().Contains(termLower) ||
                        v.CPF != null && v.CPF.Contains(term) ||
                        v.Email != null && v.Email.ToLower().Contains(termLower)
                    ))
                .OrderBy(v => v.Nome)
                .Take(pageSize)
                .Select(v => new ReferenceItem
                    {
                        Value = v.Id.ToString(),
                        Text = v.Nome,
                        Subtitle = FormatVendedorSubtitle(v.CPF, v.Email)
                    })
                .ToListAsync();
        }

        /// <summary>
        /// Busca marcas de veículos por descrição
        /// </summary>
        private async Task<List<ReferenceItem>> SearchVeiculoMarcas(string term, int pageSize)
        {
            var termLower = term.ToLower();

            return await _context.VeiculoMarcas
                .Where(m => m.Descricao.ToLower().Contains(termLower))
                .OrderBy(m => m.Descricao)
                .Take(pageSize)
                .Select(m => new ReferenceItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Descricao,
                        Subtitle = null
                    })
                .ToListAsync();
        }

        /// <summary>
        /// Busca modelos de veículos por descrição
        /// </summary>
        private async Task<List<ReferenceItem>> SearchVeiculoMarcaModelos(string term, int pageSize)
        {
            var termLower = term.ToLower();

            return await _context.VeiculoMarcaModelos
                .Include(m => m.VeiculoMarca)
                .Where(m => m.Descricao.ToLower().Contains(termLower) ||
                           m.VeiculoMarca != null && m.VeiculoMarca.Descricao.ToLower().Contains(termLower))
                .OrderBy(m => m.VeiculoMarca != null ? m.VeiculoMarca.Descricao : "")
                .ThenBy(m => m.Descricao)
                .Take(pageSize)
                .Select(m => new ReferenceItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Descricao,
                        Subtitle = m.VeiculoMarca != null ? m.VeiculoMarca.Descricao : null
                    })
                .ToListAsync();
        }

        /// <summary>
        /// Busca cores de veículos por descrição
        /// </summary>
        private async Task<List<ReferenceItem>> SearchVeiculoCores(string term, int pageSize)
        {
            var termLower = term.ToLower();

            return await _context.VeiculoCores
                .Where(c => c.Descricao.ToLower().Contains(termLower))
                .OrderBy(c => c.Descricao)
                .Take(pageSize)
                .Select(c => new ReferenceItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Descricao,
                        Subtitle = null
                    })
                .ToListAsync();
        }

        #endregion

        #region Métodos GetById por Entidade

        /// <summary>
        /// Obtém cliente por ID
        /// </summary>
        private async Task<ReferenceItem?> GetClienteById(string id)
        {
            if (!int.TryParse(id, out var clienteId))
            {
                return null;
            }

            return await _context.Clientes
                .Where(c => c.Id == clienteId)
                .Select(c => new ReferenceItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Nome,
                        Subtitle = FormatClienteSubtitle(c.CPF, c.Email, c.Telefone)
                    })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém fornecedor por ID
        /// </summary>
        private async Task<ReferenceItem?> GetFornecedorById(string id)
        {
            if (!int.TryParse(id, out var fornecedorId))
            {
                return null;
            }

            return await _context.Fornecedores
                .Where(f => f.Id == fornecedorId)
                .Select(f => new ReferenceItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Nome,
                    Subtitle = FormatFornecedorSubtitle(f.CNPJ, f.Email)
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém vendedor por ID
        /// </summary>
        private async Task<ReferenceItem?> GetVendedorById(string id)
        {
            if (!int.TryParse(id, out var vendedorId))
            {
                return null;
            }

            return await _context.Vendedores
                .Where(v => v.Id == vendedorId)
                .Select(v => new ReferenceItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Nome,
                    Subtitle = FormatVendedorSubtitle(v.CPF, v.Email)
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém marca de veículo por ID
        /// </summary>
        private async Task<ReferenceItem?> GetVeiculoMarcaById(string id)
        {
            if (!int.TryParse(id, out var marcaId))
            {
                return null;
            }

            return await _context.VeiculoMarcas
                .Where(m => m.Id == marcaId)
                .Select(m => new ReferenceItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Descricao,
                    Subtitle = null
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém modelo de veículo por ID
        /// </summary>
        private async Task<ReferenceItem?> GetVeiculoMarcaModeloById(string id)
        {
            if (!int.TryParse(id, out var modeloId))
            {
                return null;
            }

            return await _context.VeiculoMarcaModelos
                .Include(m => m.VeiculoMarca)
                .Where(m => m.Id == modeloId)
                .Select(m => new ReferenceItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Descricao,
                    Subtitle = m.VeiculoMarca != null ? m.VeiculoMarca.Descricao : null
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém cor de veículo por ID
        /// </summary>
        private async Task<ReferenceItem?> GetVeiculoCorById(string id)
        {
            if (!int.TryParse(id, out var corId))
            {
                return null;
            }

            return await _context.VeiculoCores
                .Where(c => c.Id == corId)
                .Select(c => new ReferenceItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Descricao,
                    Subtitle = null
                })
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Métodos Utilitários para Formatação

        /// <summary>
        /// Formata subtitle para cliente
        /// </summary>
        private static string FormatClienteSubtitle(string? cpf, string? email, string? telefone)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(cpf))
            {
                parts.Add($"CPF: {cpf}");
            }

            if (!string.IsNullOrEmpty(email))
            {
                parts.Add(email);
            }

            if (!string.IsNullOrEmpty(telefone))
            {
                parts.Add(telefone);
            }

            return parts.Any() ? string.Join(" • ", parts) : "Sem informações adicionais";
        }

        /// <summary>
        /// Formata subtitle para fornecedor
        /// </summary>
        private static string FormatFornecedorSubtitle(string? cnpj, string? email)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(cnpj))
            {
                parts.Add($"CNPJ: {cnpj}");
            }

            if (!string.IsNullOrEmpty(email))
            {
                parts.Add(email);
            }

            return parts.Any() ? string.Join(" • ", parts) : "Sem informações adicionais";
        }

        /// <summary>
        /// Formata subtitle para vendedor
        /// </summary>
        private static string FormatVendedorSubtitle(string? cpf, string? email)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(cpf))
            {
                parts.Add($"CPF: {cpf}");
            }

            if (!string.IsNullOrEmpty(email))
            {
                parts.Add(email);
            }

            return parts.Any() ? string.Join(" • ", parts) : "Sem informações adicionais";
        }

        #endregion
    }

    #region DTOs e Modelos de Request/Response

    /// <summary>
    /// Request para busca de referências
    /// </summary>
    public class ReferenceSearchRequest
    {
        [Required(ErrorMessage = "EntityType é obrigatório")]
        public string EntityType { get; set; } = "";

        [Required(ErrorMessage = "SearchTerm é obrigatório")]
        [MinLength(2, ErrorMessage = "SearchTerm deve ter pelo menos 2 caracteres")]
        public string SearchTerm { get; set; } = "";

        [Range(1, 50, ErrorMessage = "PageSize deve estar entre 1 e 50")]
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Request para busca por ID
    /// </summary>
    public class ReferenceGetByIdRequest
    {
        [Required(ErrorMessage = "EntityType é obrigatório")]
        public string EntityType { get; set; } = "";

        [Required(ErrorMessage = "Id é obrigatório")]
        public string Id { get; set; } = "";
    }

    /// <summary>
    /// Item de referência retornado pela API
    /// </summary>
    public class ReferenceItem
    {
        /// <summary>
        /// Valor (ID) do item
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// Texto principal para exibição
        /// </summary>
        public string Text { get; set; } = "";

        /// <summary>
        /// Texto secundário/subtitle (opcional)
        /// </summary>
        public string? Subtitle { get; set; }

        /// <summary>
        /// Informações extras (para futuras expansões)
        /// </summary>
        public Dictionary<string, object>? Extra { get; set; }
    }

    #endregion
}