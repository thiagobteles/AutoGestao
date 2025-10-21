using AutoGestao.Data;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers.Base
{
    [Authorize]
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalVeiculos = await _context.Veiculos.CountAsync(),
                TotalClientes = await _context.Clientes.CountAsync(),
                VeiculosEstoque = await _context.Veiculos.CountAsync(v => v.Situacao == EnumSituacaoVeiculo.Estoque),
                VeiculosVendidos = await _context.Veiculos.CountAsync(v => v.Situacao == EnumSituacaoVeiculo.Vendido),
                
                Aniversariantes = await _context.Clientes
                    .Where(x => x.DataNascimento.HasValue && x.DataNascimento.Value.Month == DateTime.UtcNow.Month)
                    .Take(4)
                    .ToListAsync(),
            };

            return View(dashboard);
        }
    }
}