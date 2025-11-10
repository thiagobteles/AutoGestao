using FGT.Data;
using FGT.Enumerador.Fiscal;
using FGT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FGT.Controllers.Base
{
    [Authorize]
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var mesAtual = DateTime.Now.Month;
            var anoAtual = DateTime.Now.Year;
            var dataInicioMes = new DateTime(anoAtual, mesAtual, 1);
            var dataFimMes = dataInicioMes.AddMonths(1).AddDays(-1);
            var dataLimiteVencimento = DateTime.Now.AddDays(30);

            var dashboard = new DashboardViewModel
            {
                // Primeira linha de cards
                TotalEmpresasClientes = await _context.EmpresasClientes.CountAsync(e => e.Ativo),

                NotasFiscaisEmitidas = await _context.NotasFiscais
                    .CountAsync(n => n.DataEmissao >= dataInicioMes && n.DataEmissao <= dataFimMes),

                ObrigacoesPendentes = await _context.ObrigacoesFiscais
                    .CountAsync(o => o.Status == EnumStatusObrigacao.Pendente || o.Status == EnumStatusObrigacao.EmAndamento),

                CertificadosVencendo = await _context.CertificadosDigitais
                    .CountAsync(c => c.DataValidade <= dataLimiteVencimento && c.DataValidade >= DateTime.Now),

                // Segunda linha - Faturamento
                FaturamentoMes = await _context.NotasFiscais
                    .Where(n => n.DataEmissao >= dataInicioMes && n.DataEmissao <= dataFimMes)
                    .SumAsync(n => (decimal?)n.ValorTotal) ?? 0,

                FaturamentoNFe = await _context.NotasFiscais
                    .Where(n => n.DataEmissao >= dataInicioMes && n.DataEmissao <= dataFimMes)
                    .SumAsync(n => (decimal?)n.ValorTotal) ?? 0,

                FaturamentoNFSe = 0, // Separar quando houver campo de tipo na NotaFiscal

                ObrigacoesProximas = await _context.ObrigacoesFiscais
                    .Where(o => o.DataVencimento >= DateTime.Now && o.DataVencimento <= dataLimiteVencimento)
                    .Where(o => o.Status != EnumStatusObrigacao.Entregue)
                    .OrderBy(o => o.DataVencimento)
                    .Take(5)
                    .ToListAsync(),

                // Terceira linha de cards
                LancamentosContabeisMes = await _context.LancamentosContabeis
                    .CountAsync(l => l.DataLancamento >= dataInicioMes && l.DataLancamento <= dataFimMes),

                TotalPlanoContas = await _context.PlanoContas
                    .CountAsync(p => p.ContaAnalitica),

                AliquotasConfiguradas = await _context.AliquotasImpostos.CountAsync(),

                TotalContadores = await _context.ContadoresResponsaveis.CountAsync(c => c.Ativo),

                // Dados adicionais
                TotalClientes = await _context.Clientes.CountAsync(),

                Aniversariantes = await _context.Clientes
                    .Where(x => x.DataNascimento.HasValue && x.DataNascimento.Value.Month == DateTime.UtcNow.Month)
                    .Take(4)
                    .ToListAsync(),

                VendasMes = await _context.NotasFiscais
                    .CountAsync(n => n.DataEmissao >= dataInicioMes && n.DataEmissao <= dataFimMes),

                ValorVendasMes = await _context.NotasFiscais
                    .Where(n => n.DataEmissao >= dataInicioMes && n.DataEmissao <= dataFimMes)
                    .SumAsync(n => (decimal?)n.ValorTotal) ?? 0,

                // Métricas detalhadas por regime tributário
                EmpresasSimples = await _context.EmpresasClientes
                    .CountAsync(e => e.RegimeTributario == EnumRegimeTributario.SimplesNacional),

                EmpresasLucroPresumido = await _context.EmpresasClientes
                    .CountAsync(e => e.RegimeTributario == EnumRegimeTributario.LucroPresumido),

                EmpresasLucroReal = await _context.EmpresasClientes
                    .CountAsync(e => e.RegimeTributario == EnumRegimeTributario.LucroReal),

                NotasEmitidas = await _context.NotasFiscais
                    .CountAsync(n => n.DataEmissao >= dataInicioMes && n.DataEmissao <= dataFimMes),

                NotasCanceladas = 0 // Adicionar quando houver campo de status na NotaFiscal
            };

            return View(dashboard);
        }
    }
}