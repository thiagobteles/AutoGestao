using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;
namespace AutoGestao.Controllers
{
    public class VeiculoFiliaisController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoFilial>> logger,  IReportService reportService) 
        : StandardGridController<VeiculoFilial>(context, fileStorageService, logger, reportService)
    {
    }
}