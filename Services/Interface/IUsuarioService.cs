using AutoGestao.Entidades;

namespace AutoGestao.Services.Interface
{
    public interface IUsuarioService
    {
        Task<Usuario> CriarUsuarioAsync(Usuario usuario, string senha);

        Task<Usuario?> BuscarPorEmailAsync(string email);

        Task<bool> EmailExisteAsync(string email, long? usuarioId = null);

        Task<bool> AlterarSenhaAsync(long usuarioId, string senhaAtual, string novaSenha);
    }
}