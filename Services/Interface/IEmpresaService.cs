using AutoGestao.Entidades;

namespace AutoGestao.Services.Interface
{
    public interface IEmpresaService
    {
        Task<Empresa> CriarEmpresaAsync(Empresa empresa);
    }
}