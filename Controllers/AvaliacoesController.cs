using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services;

namespace AutoGestao.Controllers
{
    public class AvaliacoesController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Avaliacao>> logger)
        : StandardGridController<Avaliacao>(context, fileStorageService, logger)
    {
    }
}