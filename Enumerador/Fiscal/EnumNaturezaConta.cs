using System.ComponentModel;

namespace FGT.Enumerador.Fiscal
{
    public enum EnumNaturezaConta
    {
        [Description("➕ Devedora")]
        Devedora = 1,

        [Description("➖ Credora")]
        Credora = 2
    }
}
