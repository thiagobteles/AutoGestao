using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

using AutoGestao.Services;
namespace AutoGestao.Controllers
{
    public class VeiculoFiliaisController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoFilial>> logger) 
        : StandardGridController<VeiculoFilial>(context, fileStorageService, logger)
    {
    }
}