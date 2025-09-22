using AutoGestao.Models.Auth;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Controllers
{
    [AllowAnonymous]
    public class LoginController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

        [HttpGet]
        public IActionResult Index()
        {
            // Se já estiver logado, redirecionar para home
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

            // Salvar token no cookie para autenticação
            Response.Cookies.Append("auth_token", result.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            TempData["SuccessMessage"] = "Login realizado com sucesso!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var usuarioId = int.Parse(User.FindFirst("NameIdentifier")?.Value ?? "0");
                await _authService.LogoutAsync(usuarioId);
            }

            Response.Cookies.Delete("auth_token");
            TempData["InfoMessage"] = "Logout realizado com sucesso!";
            return RedirectToAction("Index", "Login");
        }
    }
}