using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services;

namespace AutoGestao.Controllers
{
    public class ParcelasController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Parcela>> logger) 
        : StandardGridController<Parcela>(context, fileStorageService, logger)
    {
    }
}