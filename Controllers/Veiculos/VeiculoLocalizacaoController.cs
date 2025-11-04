using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Services.Interface;
namespace AutoGestao.Controllers.Veiculos
{
    public class VeiculoLocalizacaoController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoLocalizacao>> logger) 
        : StandardGridController<VeiculoLocalizacao>(context, fileStorageService, logger)
    {
    }
}