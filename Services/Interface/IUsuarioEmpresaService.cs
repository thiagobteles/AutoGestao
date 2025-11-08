using AutoGestao.Entidades;

namespace AutoGestao.Services.Interface
{
    public interface IUsuarioEmpresaService
    {
        /// <summary>
        /// Obt em todas as empresas vinculadas a um usuário
        /// </summary>
        Task<List<long>> GetEmpresasDoUsuarioAsync(long idUsuario);

        /// <summary>
        /// Adiciona vínculo entre usuário e empresa cliente
        /// </summary>
        Task<bool> VincularEmpresaAsync(long idUsuario, long idEmpresaCliente);

        /// <summary>
        /// Remove vínculo entre usuário e empresa cliente
        /// </summary>
        Task<bool> DesvincularEmpresaAsync(long idUsuario, long idEmpresaCliente);

        /// <summary>
        /// Verifica se usuário tem acesso a uma empresa específica
        /// </summary>
        Task<bool> TemAcessoEmpresaAsync(long idUsuario, long idEmpresaCliente);
    }
}
