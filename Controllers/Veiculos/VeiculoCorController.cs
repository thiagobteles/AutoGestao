using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers.Veiculos
{
    public class VeiculoCorController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoCor>> logger, IReportService reportService) 
        : StandardGridController<VeiculoCor>(context, fileStorageService, reportService, logger)
    {
    }
}