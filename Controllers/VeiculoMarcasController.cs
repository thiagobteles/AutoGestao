using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class VeiculoMarcasController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoMarca>> logger,  IReportService reportService) 
        : StandardGridController<VeiculoMarca>(context, fileStorageService, logger, reportService)
    {
    }
}