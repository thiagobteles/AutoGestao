using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;
namespace AutoGestao.Controllers
{
    public class DespesaTiposController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<DespesaTipo>> logger, IReportService reportService) 
        : StandardGridController<DespesaTipo>(context, fileStorageService, reportService, logger)
    {
    }
}