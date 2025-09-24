using System.ComponentModel;

namespace AutoGestao.Enumerador.Gerais
{

    public enum EnumTipoOperacaoAuditoria
    {
        [Description("Criação")]
        Create = 1,

        [Description("Alteração")]
        Update = 2,

        [Description("Exclusão")]
        Delete = 3,

        [Description("Login")]
        Login = 4,

        [Description("Logout")]
        Logout = 5,

        [Description("Falha de Login")]
        LoginFailed = 6,

        [Description("Alteração de Senha")]
        PasswordChange = 7,

        [Description("Visualização")]
        View = 8,

        [Description("Exportação")]
        Export = 9,

        [Description("Importação")]
        Import = 10,

        [Description("Unknown")]
        Unknown = 11
    }
}