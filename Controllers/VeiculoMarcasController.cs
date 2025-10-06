using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services;

namespace AutoGestao.Controllers
{
    public class VeiculoMarcasController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoMarca>> logger) 
        : StandardGridController<VeiculoMarca>(context, fileStorageService, logger)
    {
    }
}