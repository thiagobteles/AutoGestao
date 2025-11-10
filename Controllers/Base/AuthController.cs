using FGT.Models.Auth;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGT.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos"
                });
            }

            var result = await _authService.LoginAsync(request);

            return !result.Sucesso ? (ActionResult<LoginResponse>)Unauthorized(result) : (ActionResult<LoginResponse>)Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var usuarioId = int.Parse(User.FindFirst("NameIdentifier")?.Value ?? "0");
            await _authService.LogoutAsync(usuarioId);
            return Ok(new { Mensagem = "Logout realizado com sucesso" });
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok(new
            {
                Valido = true,
                Usuario = new
                {
                    Id = User.FindFirst("NameIdentifier")?.Value,
                    Nome = User.FindFirst("Name")?.Value,
                    Email = User.FindFirst("Email")?.Value,
                    Perfil = User.FindFirst("Perfil")?.Value
                }
            });
        }
    }
}