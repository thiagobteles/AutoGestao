using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;

namespace AutoGestao.Controllers
{
    public class FornecedoresController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Fornecedor>> logger) 
        : StandardGridController<Fornecedor>(context, fileStorageService, logger)
    {
    }
}