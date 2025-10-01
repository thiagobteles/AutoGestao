using AutoGestao.Models.Auth;

namespace AutoGestao.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);

        bool ValidateToken(string token);

        Task LogoutAsync(int usuarioId);

        string[] GetRolesByPerfil(string perfil);
    }
}