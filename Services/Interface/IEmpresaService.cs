using AutoGestao.Entidades.Base;

namespace AutoGestao.Services.Interface
{
    public interface IEmpresaService
    {
        Task<Empresa> CriarEmpresaAsync(Empresa empresa);
    }
}