using AutoGestao.Data;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace AutoGestao.ViewComponents
{
    public class SeletorEmpresaViewComponent : ViewComponent
    {
        private readonly IUsuarioEmpresaService _usuarioEmpresaService;
        private readonly ApplicationDbContext _context;

        public SeletorEmpresaViewComponent(IUsuarioEmpresaService usuarioEmpresaService, ApplicationDbContext context)
        {
            _usuarioEmpresaService = usuarioEmpresaService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Verificar se usuário está autenticado
            if (!UserClaimsPrincipal.Identity?.IsAuthenticated ?? true)
            {
                return Content(string.Empty);
            }

            // Obter ID do usuário
            var usuarioIdClaim = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return Content(string.Empty);
            }

            // Obter empresa ativa
            var empresaAtivaIdClaim = UserClaimsPrincipal.FindFirst("EmpresaClienteId")?.Value;
            long? empresaAtivaId = null;
            if (long.TryParse(empresaAtivaIdClaim, out var empresaId))
            {
                empresaAtivaId = empresaId;
            }

            // Buscar empresas vinculadas ao usuário
            var empresasIds = await _usuarioEmpresaService.GetEmpresasDoUsuarioAsync(usuarioId);

            // Se não tem empresas vinculadas, não exibir o seletor
            if (empresasIds.Count <= 1)
            {
                return Content(string.Empty);
            }

            // Buscar detalhes das empresas
            var empresas = await _context.EmpresasClientes
                .Where(e => empresasIds.Contains(e.Id))
                .Select(e => new EmpresaViewModel
                {
                    Id = e.Id,
                    RazaoSocial = e.RazaoSocial,
                    CNPJ = e.CNPJ,
                    NomeFantasia = e.NomeFantasia
                })
                .OrderBy(e => e.RazaoSocial)
                .ToListAsync();

            var model = new SeletorEmpresaViewModel
            {
                EmpresaAtivaId = empresaAtivaId,
                Empresas = empresas
            };

            return View(model);
        }
    }

    public class SeletorEmpresaViewModel
    {
        public long? EmpresaAtivaId { get; set; }
        public List<EmpresaViewModel> Empresas { get; set; } = new();
    }

    public class EmpresaViewModel
    {
        public long Id { get; set; }
        public string RazaoSocial { get; set; } = "";
        public string? CNPJ { get; set; }
        public string? NomeFantasia { get; set; }

        public string DisplayText => string.IsNullOrEmpty(NomeFantasia)
            ? RazaoSocial
            : $"{NomeFantasia} - {RazaoSocial}";
    }
}
