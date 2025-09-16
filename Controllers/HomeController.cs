using AutoGestao.Data;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoGestao.Enumerador.Veiculo;

namespace AutoGestao.Controllers
{
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
                VendasMes = await _context.Vendas.CountAsync(v => v.DataVenda.Month == DateTime.Now.Month),
                ValorVendasMes = await _context.Vendas
                    .Where(v => v.DataVenda.Month == DateTime.Now.Month)
                    .SumAsync(v => v.ValorVenda)
            };

            return View(dashboard);
        }

        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return Json(new { success = canConnect, message = canConnect ? "Conexão OK!" : "Falha na conexão" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}