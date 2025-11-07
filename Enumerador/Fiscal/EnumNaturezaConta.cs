using System.ComponentModel;

namespace AutoGestao.Enumerador.Fiscal
{
    public enum EnumNaturezaConta
    {
        [Description("➕ Devedora")]
        Devedora = 1,

        [Description("➖ Credora")]
        Credora = 2
    }
}
