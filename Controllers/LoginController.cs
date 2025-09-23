using AutoGestao.Models.Auth;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoGestao.Controllers
{
    [AllowAnonymous]
    public class LoginController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

        [HttpGet]
        public IActionResult Index()
        {
            // 🔧 DEBUG: Verificar estado da autenticação
            Console.WriteLine($"DEBUG: IsAuthenticated = {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"DEBUG: User Name = {User.Identity?.Name}");

            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 🔧 VALIDAR CREDENCIAIS DIRETAMENTE NO BANCO
            var result = await _authService.LoginAsync(model);

            if (!result.Sucesso)
            {
                ModelState.AddModelError(string.Empty, result.Mensagem ?? "Erro ao realizar login");
                return View(model);
            }

            // 🔧 CRIAR CLAIMS MANUALMENTE
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.Usuario!.Id.ToString()),
                new Claim(ClaimTypes.Name, result.Usuario.Nome),
                new Claim(ClaimTypes.Email, result.Usuario.Email),
                new Claim("Perfil", result.Usuario.Perfil)
            };

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

            // 🔧 DEBUG: Verificar após login
            Console.WriteLine($"DEBUG: Login efetuado para {result.Usuario.Nome}");
            Console.WriteLine($"DEBUG: Claims criados: {claims.Count}");

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
    }
}