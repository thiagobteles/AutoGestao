using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class VeiculoMarcaModelosController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoMarcaModelo>> logger) 
        : StandardGridController<VeiculoMarcaModelo>(context, fileStorageService, logger)
    {
    }
}