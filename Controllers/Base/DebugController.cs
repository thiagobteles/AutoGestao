using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGT.Controllers.Base
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : Controller
    {
        /// <summary>
        /// Endpoint para visualizar todas as claims do usuário logado
        /// Útil para debug de problemas de autenticação e autorização
        /// </summary>
        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            }).ToList();

            var empresaClienteIdClaim = User.FindFirst("EmpresaClienteId")?.Value;

            return Ok(new
            {
                TotalClaims = claims.Count,
                EmpresaClienteIdClaim = empresaClienteIdClaim ?? "NULL",
                AllClaims = claims
            });
        }
    }
}
