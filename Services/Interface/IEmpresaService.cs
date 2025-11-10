using FGT.Entidades.Base;

namespace FGT.Services.Interface
{
    public interface IEmpresaService
    {
        Task<Empresa> CriarEmpresaAsync(Empresa empresa);
    }
}