namespace AutoGestao.Models.Auth
{
    public class UsuarioInfo
    {
        public long Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Perfil { get; set; }

        public long IdEmpresa { get; set; }

        public long? IdEmpresaCliente { get; set; }

        /// <summary>
        /// Lista de IDs de todas as empresas que o usuário tem acesso
        /// </summary>
        public List<long> EmpresasVinculadas { get; set; } = [];

        public string[] Roles { get; set; } = [];
    }
}