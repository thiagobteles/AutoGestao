using System.ComponentModel;

namespace AutoGestao.Enumerador.Fiscal
{
    public enum EnumTipoImposto
    {
        [Description("💰 ICMS")]
        ICMS = 1,

        [Description("💵 ISS")]
        ISS = 2,

        [Description("📊 IPI")]
        IPI = 3,

        [Description("🔵 PIS")]
        PIS = 4,

        [Description("🟢 COFINS")]
        COFINS = 5,

        [Description("📈 IRPJ")]
        IRPJ = 6,

        [Description("💼 CSLL")]
        CSLL = 7,

        [Description("🔷 INSS")]
        INSS = 8,

        [Description("⚡ ICMS-ST")]
        ICMSST = 9,

        [Description("🎯 Simples Nacional")]
        SimplesNacional = 10
    }
}
