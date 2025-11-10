namespace FGT.Models.Auth
{
    public class LoginResponse
    {
        public bool Sucesso { get; set; }
        public string? Token { get; set; }
        public string? Mensagem { get; set; }
        public UsuarioInfo? Usuario { get; set; }
    }
}