namespace AutoGestao.Models.Auth
{
    public class UsuarioInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public string Perfil { get; set; } = "";
        public string[] Roles { get; set; } = [];
    }
}