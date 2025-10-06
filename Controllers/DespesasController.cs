using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class DespesasController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Despesa>> logger)
        : StandardGridController<Despesa>(context, fileStorageService, logger)
    {
    }
}