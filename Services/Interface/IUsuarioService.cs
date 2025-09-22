using AutoGestao.Entidades;

namespace AutoGestao.Services.Interface
{
    public interface IUsuarioService
    {
        Task<Usuario> CriarUsuarioAsync(Usuario usuario, string senha);

        Task<Usuario?> BuscarPorEmailAsync(string email);

        Task<bool> EmailExisteAsync(string email, int? usuarioId = null);

        Task<bool> AlterarSenhaAsync(int usuarioId, string senhaAtual, string novaSenha);
    }
}