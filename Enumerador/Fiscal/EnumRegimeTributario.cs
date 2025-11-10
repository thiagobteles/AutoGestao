using System.ComponentModel;

namespace FGT.Enumerador.Fiscal
{
    public enum EnumRegimeTributario
    {
        [Description("🟢 Simples Nacional")]
        SimplesNacional = 1,

        [Description("🔵 Lucro Presumido")]
        LucroPresumido = 2,

        [Description("🟣 Lucro Real")]
        LucroReal = 3,

        [Description("🟡 MEI")]
        MEI = 4
    }
}
