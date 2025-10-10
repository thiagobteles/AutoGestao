using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;
namespace AutoGestao.Controllers
{
    public class VeiculoFotoController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoFoto>> logger, IReportService reportService) 
        : StandardGridController<VeiculoFoto>(context, fileStorageService, reportService, logger)
    {
    }
}