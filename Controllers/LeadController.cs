using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class LeadController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Lead>> logger, IReportService reportService) 
        : StandardGridController<Lead>(context, fileStorageService, reportService, logger)
    {
    }
}