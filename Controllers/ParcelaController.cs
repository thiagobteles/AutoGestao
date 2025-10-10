using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class ParcelaController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Parcela>> logger, IReportService reportService) 
        : StandardGridController<Parcela>(context, fileStorageService, reportService, logger)
    {
    }
}