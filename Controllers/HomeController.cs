using AutoGestao.Data;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoGestao.Controllers
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
                VeiculosEstoque = await _context.Veiculos.CountAsync(v => v.Situacao == EnumSituacaoVeiculo.Estoque),
                VeiculosVendidos = await _context.Veiculos.CountAsync(v => v.Situacao == EnumSituacaoVeiculo.Vendido),
                TotalClientes = await _context.Clientes.CountAsync(),
                VendasMes = await _context.Vendas.CountAsync(v => v.DataVenda.Month == DateTime.UtcNow.Month),
                ValorVendasMes = await _context.Vendas
                    .Where(v => v.DataVenda.Month == DateTime.UtcNow.Month)
                    .SumAsync(v => v.ValorVenda)
            };

            return View(dashboard);
        }
    }
}