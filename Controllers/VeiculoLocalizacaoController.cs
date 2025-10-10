using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;
namespace AutoGestao.Controllers
{
    public class VeiculoLocalizacaoController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoLocalizacao>> logger, IReportService reportService) 
        : StandardGridController<VeiculoLocalizacao>(context, fileStorageService, reportService, logger)
    {
    }
}