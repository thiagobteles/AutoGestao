using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers.Veiculos
{
    public class VeiculoMarcaModeloController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoMarcaModelo>> logger, IReportService reportService) 
        : StandardGridController<VeiculoMarcaModelo>(context, fileStorageService, reportService, logger)
    {
    }
}