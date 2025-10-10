using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class TarefaController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Tarefa>> logger, IReportService reportService) 
        : StandardGridController<Tarefa>(context, fileStorageService, reportService, logger)
    {
    }
}