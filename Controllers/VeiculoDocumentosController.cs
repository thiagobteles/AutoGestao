using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class VeiculoDocumentosController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoDocumento>> logger) 
        : StandardGridController<VeiculoDocumento>(context, fileStorageService, logger)
    {
    }
}