using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Gerais
{
    public enum EnumPerfilUsuario
    {
        [Icone("👑")]
        [Description("Administrador")]
        Admin = 1,

        [Icone("👨‍💼")]
        [Description("Gerente")]
        Gerente = 2,

        [Icone("🤝")]
        [Description("Vendedor")]
        Vendedor = 3,

        [Icone("💰")]
        [Description("Financeiro")]
        Financeiro = 4,

        [Icone("👁️")]
        [Description("Visualizador")]
        Visualizador = 5
    }
}