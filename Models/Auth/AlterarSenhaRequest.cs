namespace AutoGestao.Models.Auth
{
    public class AlterarSenhaRequest
    {
        public int UsuarioId { get; set; }
        public string SenhaAtual { get; set; } = "";
        public string NovaSenha { get; set; } = "";
        public string ConfirmarSenha { get; set; } = "";
    }
}