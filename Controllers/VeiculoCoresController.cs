using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services;

namespace AutoGestao.Controllers
{
    public class VeiculoCoresController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoCor>> logger) 
        : StandardGridController<VeiculoCor>(context, fileStorageService, logger)
    {
    }
}