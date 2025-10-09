using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class DespesasController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Despesa>> logger,  IReportService reportService)
        : StandardGridController<Despesa>(context, fileStorageService, logger, reportService)
    {
    }
}