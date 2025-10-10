using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class VendaController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Venda>> logger, IReportService reportService) 
        : StandardGridController<Venda>(context, fileStorageService, reportService, logger)
    {
    }
}