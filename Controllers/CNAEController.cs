using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Controllers
{
    public class CNAEController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<CNAE>> logger)
        : StandardGridController<CNAE>(context, fileStorageService, logger)
    {
    }
}
