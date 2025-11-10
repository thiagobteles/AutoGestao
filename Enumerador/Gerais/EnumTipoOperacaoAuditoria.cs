using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador.Gerais
{
    public enum EnumTipoOperacaoAuditoria
    {
        [Icone("fas fa-plus-circle")]
        [Description("Criação")]
        Create = 1,

        [Icone("fas fa-edit")]
        [Description("Alteração")]
        Update = 2,

        [Icone("fas fa-trash-alt")]
        [Description("Exclusão")]
        Delete = 3,

        [Icone("fas fa-sign-in-alt")]
        [Description("Login")]
        Login = 4,

        [Icone("fas fa-sign-out-alt")]
        [Description("Logout")]
        Logout = 5,

        [Icone("fas fa-exclamation-triangle")]
        [Description("Falha de Login")]
        LoginFailed = 6,

        [Icone("fas fa-key")]
        [Description("Alteração de Senha")]
        PasswordChange = 7,

        [Icone("fas fa-eye")]
        [Description("Visualização")]
        View = 8,

        [Icone("fas fa-file-export")]
        [Description("Exportação")]
        Export = 9,

        [Icone("fas fa-file-import")]
        [Description("Importação")]
        Import = 10,

        [Icone("fas fa-file-pdf")]
        [Description("Impressão de Relatório")]
        PrintReport = 11,

        [Icone("fas fa-bolt")]
        [Description("Execução de Ação")]
        Action = 12,

        [Icone("fas fa-question-circle")]
        [Description("Não mapeado")]
        Unknown = 99
    }
}