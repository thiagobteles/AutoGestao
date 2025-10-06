using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;
namespace AutoGestao.Controllers
{
    public class VeiculoFotosController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoFoto>> logger) 
        : StandardGridController<VeiculoFoto>(context, fileStorageService, logger)
    {
    }
}