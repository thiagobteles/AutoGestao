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
            //var vendasMesAnterior = await _context.Vendas.Where(x => x.DataVenda.Month == DateTime.UtcNow.Month - 1).SumAsync(x => x.ValorVenda);
            //var vendasMesAtual = await _context.Vendas.Where(x => x.DataVenda.Month == DateTime.UtcNow.Month).SumAsync(x => x.ValorVenda);

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

                //ComparativoComMesAnterior = vendasMesAtual - vendasMesAnterior,

                //VendasMes = await _context.Vendas.CountAsync(v => v.DataVenda.Month == DateTime.UtcNow.Month),

                //ValorVendasMes = await _context.Vendas
                //    .Where(v => v.DataVenda.Month == DateTime.UtcNow.Month)
                //    .SumAsync(v => v.ValorVenda),

                //UltimasVendas = await _context.Vendas
                //    .Include(x => x.Vendedor)
                //    .Include(x => x.Veiculo)
                //    .Include(x => x.Veiculo.VeiculoMarca)
                //    .Include(x => x.Veiculo.VeiculoMarcaModelo)
                //    .OrderByDescending(x => x.DataVenda)
                //    .Take(4)
                //    .ToListAsync()

                ComparativoComMesAnterior = 2,
                VendasMes = 30,
                ValorVendasMes = (decimal)123530.50,
                UltimasVendas = [1, 2, 3]
            };

            return View(dashboard);
        }
    }
}