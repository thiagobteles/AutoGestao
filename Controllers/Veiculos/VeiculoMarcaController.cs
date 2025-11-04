using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers.Veiculos
{
    public class VeiculoMarcaController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoMarca>> logger) 
        : StandardGridController<VeiculoMarca>(context, fileStorageService, logger)
    {
    }
}