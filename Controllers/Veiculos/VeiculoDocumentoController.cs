using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers.Veiculos
{
    public class VeiculoDocumentoController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoDocumento>> logger) 
        : StandardGridController<VeiculoDocumento>(context, fileStorageService, logger)
    {
    }
}