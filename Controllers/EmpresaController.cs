using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class EmpresaController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Empresa>> logger, IReportService reportService) 
        : StandardGridController<Empresa>(context, fileStorageService, reportService, logger)
    {
    }
}