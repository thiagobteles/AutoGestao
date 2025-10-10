using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class VeiculoDocumentoController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoDocumento>> logger, IReportService reportService) 
        : StandardGridController<VeiculoDocumento>(context, fileStorageService, reportService, logger)
    {
    }
}