namespace AutoGestao.Models.Auth
{
    public class UsuarioInfo
    {
        public long Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Perfil { get; set; }

        public long IdEmpresa { get; set; }

        public string[] Roles { get; set; } = [];
    }
}