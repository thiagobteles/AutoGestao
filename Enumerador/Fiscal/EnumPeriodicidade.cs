using System.ComponentModel;

namespace FGT.Enumerador.Fiscal
{
    public enum EnumPeriodicidade
    {
        [Description("📅 Mensal")]
        Mensal = 1,

        [Description("📆 Trimestral")]
        Trimestral = 2,

        [Description("📊 Semestral")]
        Semestral = 3,

        [Description("📈 Anual")]
        Anual = 4,

        [Description("⚡ Eventual")]
        Eventual = 5,

        [Description("📋 Diária")]
        Diaria = 6,

        [Description("🔄 Semanal")]
        Semanal = 7,

        [Description("📌 Quinzenal")]
        Quinzenal = 8
    }
}
