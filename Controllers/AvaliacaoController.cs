using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class AvaliacaoController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Avaliacao>> logger, IReportService reportService)
        : StandardGridController<Avaliacao>(context, fileStorageService, reportService, logger)
    {
    }
}