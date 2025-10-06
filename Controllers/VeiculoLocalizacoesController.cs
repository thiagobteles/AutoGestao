using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;

using AutoGestao.Services;
namespace AutoGestao.Controllers
{
    public class VeiculoLocalizacoesController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoLocalizacao>> logger) 
        : StandardGridController<VeiculoLocalizacao>(context, fileStorageService, logger)
    {
    }
}