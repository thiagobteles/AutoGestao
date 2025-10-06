using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

using AutoGestao.Services;
namespace AutoGestao.Controllers
{
    public class DespesaTiposController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<DespesaTipo>> logger) 
        : StandardGridController<DespesaTipo>(context, fileStorageService, logger)
    {
    }
}