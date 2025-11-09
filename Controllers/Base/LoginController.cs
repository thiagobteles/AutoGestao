using AutoGestao.Models.Auth;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace AutoGestao.Controllers.Base
{
    [AllowAnonymous]
    public class LoginController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

        [HttpGet]
        public IActionResult Index()
        {
            return User.Identity?.IsAuthenticated == true 
                ? RedirectToAction("Index", "Home")
                : View(new LoginRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);
            if (!result.Sucesso)
            {
                ModelState.AddModelError(string.Empty, result.Mensagem ?? "Erro ao realizar login");
                return View(model);
            }

            // 🔧 CRIAR CLAIMS MANUALMENTE
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, result.Usuario!.Id.ToString()),
                new(ClaimTypes.Name, result.Usuario.Nome),
                new(ClaimTypes.Email, result.Usuario.Email),
                new("IdEmpresa", result.Usuario.IdEmpresa.ToString()),
                new("Perfil", result.Usuario.Perfil)
            };

            // Adicionar EmpresaClienteId se o usuário tiver vínculo (empresa padrão ou primeira da lista)
            if (result.Usuario.IdEmpresaCliente.HasValue)
            {
                claims.Add(new Claim("EmpresaClienteId", result.Usuario.IdEmpresaCliente.Value.ToString()));
            }
            else if (result.Usuario.EmpresasVinculadas?.Count > 0)
            {
                // Se não tem empresa padrão mas tem empresas vinculadas, usar a primeira
                claims.Add(new Claim("EmpresaClienteId", result.Usuario.EmpresasVinculadas[0].ToString()));
            }

            // Adicionar lista de empresas vinculadas como JSON
            if (result.Usuario.EmpresasVinculadas?.Count > 0)
            {
                var empresasJson = JsonSerializer.Serialize(result.Usuario.EmpresasVinculadas);
                claims.Add(new Claim("EmpresasVinculadas", empresasJson));
            }

            // Adicionar roles
            foreach (var role in result.Usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 🔧 FAZER LOGIN NO SISTEMA DE COOKIES
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = model.LembrarMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            TempData["SuccessMessage"] = $"Bem-vindo, {result.Usuario.Nome}!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _authService.LogoutAsync(usuarioId);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["InfoMessage"] = "Logout realizado com sucesso!";
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> TrocarEmpresa([FromBody] TrocarEmpresaRequest request)
        {
            // Verificar autenticação manualmente já que a classe é [AllowAnonymous]
            if (User.Identity?.IsAuthenticated != true)
            {
                return Json(new { success = false, message = "Usuário não autenticado" });
            }

            // Obter empresas vinculadas do claim
            var empresasVinculadasClaim = User.FindFirst("EmpresasVinculadas")?.Value;
            if (string.IsNullOrEmpty(empresasVinculadasClaim))
            {
                return Json(new { success = false, message = "Usuário não possui empresas vinculadas" });
            }

            // Deserializar lista de empresas
            var empresasVinculadas = JsonSerializer.Deserialize<List<long>>(empresasVinculadasClaim);
            if (empresasVinculadas == null || !empresasVinculadas.Contains(request.IdEmpresaCliente))
            {
                return Json(new { success = false, message = "Empresa selecionada não está vinculada ao usuário" });
            }

            // Obter todos os claims atuais exceto EmpresaClienteId
            var currentClaims = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type != "EmpresaClienteId")
                .ToList();

            // Adicionar novo claim de EmpresaClienteId
            var newClaims = new List<Claim>(currentClaims)
            {
                new Claim("EmpresaClienteId", request.IdEmpresaCliente.ToString())
            };

            // Recriar a identidade com os novos claims
            var claimsIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Obter propriedades de autenticação atuais
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            // Re-fazer sign-in com os novos claims
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            return Json(new { success = true, message = "Empresa alterada com sucesso" });
        }

        [HttpGet]
        public async Task<IActionResult> ObterEmpresasVinculadas()
        {
            // Verificar autenticação manualmente já que a classe é [AllowAnonymous]
            if (User.Identity?.IsAuthenticated != true)
            {
                return Json(new { success = false, message = "Usuário não autenticado" });
            }

            // Verificar se é admin - admin não precisa seletor de empresas
            if (User.IsInRole("Admin"))
            {
                return Json(new { success = true, isAdmin = true, empresas = new List<object>() });
            }

            var empresasVinculadasClaim = User.FindFirst("EmpresasVinculadas")?.Value;
            if (string.IsNullOrEmpty(empresasVinculadasClaim))
            {
                return Json(new { success = true, empresas = new List<object>() });
            }

            var empresasIds = JsonSerializer.Deserialize<List<long>>(empresasVinculadasClaim);
            if (empresasIds == null || empresasIds.Count == 0)
            {
                return Json(new { success = true, empresas = new List<object>() });
            }

            var empresaAtualId = User.FindFirst("EmpresaClienteId")?.Value;

            // Buscar informações das empresas
            var empresasInfo = await _authService.ObterEmpresasPorIdsAsync(empresasIds);

            var resultado = empresasInfo.Select(e => new
            {
                id = e.Id,
                nome = e.RazaoSocial ?? e.NomeFantasia ?? $"Empresa #{e.Id}",
                cnpj = e.CNPJ,
                isSelecionada = e.Id.ToString() == empresaAtualId
            }).ToList();

            return Json(new { success = true, empresas = resultado, empresaAtualId });
        }
    }

    public class TrocarEmpresaRequest
    {
        public long IdEmpresaCliente { get; set; }
    }
}