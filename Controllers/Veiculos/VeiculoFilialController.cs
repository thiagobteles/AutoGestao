using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;
namespace AutoGestao.Controllers.Veiculos
{
    public class VeiculoFilialController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoFilial>> logger, IReportService reportService) 
        : StandardGridController<VeiculoFilial>(context, fileStorageService, reportService, logger)
    {
    }
}