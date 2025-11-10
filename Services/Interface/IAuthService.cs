using FGT.Entidades;
using FGT.Models.Auth;

namespace FGT.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);

        bool ValidateToken(string token);

        Task LogoutAsync(int usuarioId);

        string[] GetRolesByPerfil(string perfil);

        Task<List<EmpresaCliente>> ObterEmpresasPorIdsAsync(List<long> ids);
    }
}