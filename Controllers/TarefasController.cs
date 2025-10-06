using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services;

namespace AutoGestao.Controllers
{
    public class TarefasController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Tarefa>> logger) 
        : StandardGridController<Tarefa>(context, fileStorageService, logger)
    {
    }
}