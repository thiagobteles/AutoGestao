using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class CertificadoDigitalController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<CertificadoDigital>> logger)
        : StandardGridController<CertificadoDigital>(context, fileStorageService, logger)
    {
        protected override IQueryable<CertificadoDigital> GetBaseQuery()
        {
            return base.GetBaseQuery().Include(c => c.EmpresaCliente);
        }
    }
}
