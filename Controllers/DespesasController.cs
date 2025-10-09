using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    [Authorize]
    public class DespesasController(ApplicationDbContext context, IFileStorageService fileStorageService, IReportService reportService, ILogger<DespesasController> logger) 
        : StandardGridController<Despesa>(context, fileStorageService, reportService, logger)
    {
        protected override IQueryable<Despesa> GetBaseQuery()
        {
            return _context.Set<Despesa>()
                .Include(d => d.Veiculo)
                .Include(d => d.DespesaTipo)
                .Include(d => d.Fornecedor)
                .Where(d => d.IdEmpresa == GetCurrentEmpresaId());
        }

        protected override IQueryable<Despesa> ApplyFilters(IQueryable<Despesa> query, Dictionary<string, object> filters)
        {
            if (filters.TryGetValue("IdVeiculo", out var idVeiculo) && long.TryParse(idVeiculo.ToString(), out var veiculoId) && veiculoId > 0)
            {
                query = query.Where(d => d.IdVeiculo == veiculoId);
            }

            if (filters.TryGetValue("Status", out var status) && int.TryParse(status.ToString(), out var statusInt))
            {
                query = query.Where(d => (int)d.Status == statusInt);
            }

            if (filters.TryGetValue("DataInicio", out var dataInicio) && DateTime.TryParse(dataInicio.ToString(), out var dtInicio))
            {
                query = query.Where(d => d.DataDespesa >= dtInicio);
            }

            if (filters.TryGetValue("DataFim", out var dataFim) && DateTime.TryParse(dataFim.ToString(), out var dtFim))
            {
                query = query.Where(d => d.DataDespesa <= dtFim);
            }

            return query;
        }

        [HttpGet]
        public override async Task<IActionResult> Create()
        {
            var idVeiculo = Request.Query["IdVeiculo"].ToString();
            var isModal = Request.Query["modal"].ToString() == "true";

            var entity = new Despesa
            {
                IdEmpresa = GetCurrentEmpresaId(), // CRÍTICO
                DataDespesa = DateTime.Now,
                Status = Enumerador.EnumStatusDespesa.Pendente
            };

            if (!string.IsNullOrEmpty(idVeiculo) && long.TryParse(idVeiculo, out var veiculoId))
            {
                entity.IdVeiculo = veiculoId;
            }

            if (isModal)
            {
                var formViewModel = await BuildFormViewModelAsync(entity, "Create");
                return PartialView("_ModalForm", formViewModel);
            }

            return await base.Create();
        }

        [HttpGet]
        public override async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var isModal = Request.Query["modal"].ToString() == "true";
            var entity = await GetBaseQuery().FirstOrDefaultAsync(d => d.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            // LOG
            _logger?.LogInformation("=== EDIT DESPESA ===");
            _logger?.LogInformation("Id: {Id}, IdEmpresa: {IdEmpresa}", entity.Id, entity.IdEmpresa);

            if (isModal)
            {
                var formViewModel = await BuildFormViewModelAsync(entity, "Edit");
                return PartialView("_ModalForm", formViewModel);
            }

            return await base.Edit(id);
        }
    }
}