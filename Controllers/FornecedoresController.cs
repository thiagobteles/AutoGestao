using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class FornecedoresController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Fornecedor>> logger, IReportService reportService) 
        : StandardGridController<Fornecedor>(context, fileStorageService, reportService, logger)
    {
    }
}