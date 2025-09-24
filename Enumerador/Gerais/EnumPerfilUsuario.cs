using System.ComponentModel;

namespace AutoGestao.Enumerador.Gerais
{
    public enum EnumPerfilUsuario
    {
        [Description("Administrador")]
        Admin = 1,
        
        [Description("Gerente")]
        Gerente = 2,
        
        [Description("Vendedor")]
        Vendedor = 3,
        
        [Description("Financeiro")]
        Financeiro = 4,
        
        [Description("Visualizador")]
        Visualizador = 5
    }
}