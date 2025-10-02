using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Gerais
{
    public enum EnumTipoOperacaoAuditoria
    {
        [Icone("➕")]
        [Description("Criação")]
        Create = 1,

        [Icone("✏️")]
        [Description("Alteração")]
        Update = 2,

        [Icone("🗑️")]
        [Description("Exclusão")]
        Delete = 3,

        [Icone("🔓")]
        [Description("Login")]
        Login = 4,

        [Icone("🔒")]
        [Description("Logout")]
        Logout = 5,

        [Icone("⚠️")]
        [Description("Falha de Login")]
        LoginFailed = 6,

        [Icone("🔑")]
        [Description("Alteração de Senha")]
        PasswordChange = 7,

        [Icone("👁️")]
        [Description("Visualização")]
        View = 8,

        [Icone("📤")]
        [Description("Exportação")]
        Export = 9,

        [Icone("📥")]
        [Description("Importação")]
        Import = 10,

        [Icone("❓")]
        [Description("Unknown")]
        Unknown = 11
    }
}