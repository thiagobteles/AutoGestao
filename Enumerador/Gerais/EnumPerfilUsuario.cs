using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Gerais
{
    public enum EnumPerfilUsuario
    {
        [Icone("fas fa-crown")]
        [Description("Administrador")]
        Admin = 1,

        [Icone("fas fa-user-tie")]
        [Description("Gerente")]
        Gerente = 2,

        [Icone("fas fa-handshake")]
        [Description("Vendedor")]
        Vendedor = 3,

        [Icone("fas fa-money-bill-wave")]
        [Description("Financeiro")]
        Financeiro = 4,

        [Icone("fas fa-eye")]
        [Description("Visualizador")]
        Visualizador = 5
    }
}