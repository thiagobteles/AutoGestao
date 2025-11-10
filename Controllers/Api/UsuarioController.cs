using FGT.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FGT.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioEmpresaService usuarioEmpresaService, ILogger<UsuarioController> logger) : ControllerBase
    {
        private readonly IUsuarioEmpresaService _usuarioEmpresaService = usuarioEmpresaService;
        private readonly ILogger<UsuarioController> _logger = logger;

        /// <summary>
        /// Troca a empresa ativa do usuário logado
        /// </summary>
        [HttpPost("trocar-empresa")]
        public async Task<IActionResult> TrocarEmpresa([FromBody] TrocarEmpresaRequest request)
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(usuarioIdClaim, out var usuarioId))
                {
                    return Unauthorized(new { sucesso = false, mensagem = "Usuário não autenticado" });
                }

                // Verificar se o usuário tem acesso a essa empresa
                var temAcesso = await _usuarioEmpresaService.TemAcessoEmpresaAsync(usuarioId, request.IdEmpresaCliente);
                if (!temAcesso)
                {
                    _logger.LogWarning("⚠️ Usuário {UsuarioId} tentou trocar para empresa {EmpresaId} sem permissão",
                        usuarioId, request.IdEmpresaCliente);
                    return Forbid();
                }

                _logger.LogInformation("🔄 Usuário {UsuarioId} trocando empresa ativa para {EmpresaId}",
                    usuarioId, request.IdEmpresaCliente);

                // Obter claims atuais
                var claims = User.Claims.ToList();

                // Remover claim antiga de EmpresaClienteId
                var claimEmpresaAtual = claims.FirstOrDefault(c => c.Type == "EmpresaClienteId");
                if (claimEmpresaAtual != null)
                {
                    claims.Remove(claimEmpresaAtual);
                }

                // Adicionar nova claim
                claims.Add(new Claim("EmpresaClienteId", request.IdEmpresaCliente.ToString()));

                // Recriar o cookie de autenticação com as novas claims
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                _logger.LogInformation("✅ Empresa ativa alterada com sucesso para {EmpresaId}", request.IdEmpresaCliente);

                return Ok(new { sucesso = true, mensagem = "Empresa alterada com sucesso", empresaId = request.IdEmpresaCliente });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao trocar empresa ativa");
                return StatusCode(500, new { sucesso = false, mensagem = "Erro ao trocar empresa" });
            }
        }

        /// <summary>
        /// Retorna as empresas vinculadas ao usuário logado
        /// </summary>
        [HttpGet("empresas-vinculadas")]
        public async Task<IActionResult> GetEmpresasVinculadas()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(usuarioIdClaim, out var usuarioId))
                {
                    return Unauthorized();
                }

                var empresasIds = await _usuarioEmpresaService.GetEmpresasDoUsuarioAsync(usuarioId);

                return Ok(new { sucesso = true, empresas = empresasIds });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar empresas vinculadas");
                return StatusCode(500, new { sucesso = false, mensagem = "Erro ao buscar empresas" });
            }
        }
    }

    public class TrocarEmpresaRequest
    {
        public long IdEmpresaCliente { get; set; }
    }
}