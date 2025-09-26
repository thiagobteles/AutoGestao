using AutoGestao.Models.Auth;

namespace AutoGestao.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<bool> ValidateTokenAsync(string token);

        Task LogoutAsync(int usuarioId);

        string[] GetRolesByPerfil(string perfil);
    }
}