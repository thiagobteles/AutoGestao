using System.ComponentModel;

namespace FGT.Enumerador.Fiscal
{
    public enum EnumStatusNotaFiscal
    {
        [Description("📝 Rascunho")]
        Rascunho = 0,

        [Description("✅ Emitida")]
        Emitida = 1,

        [Description("❌ Cancelada")]
        Cancelada = 2,

        [Description("⚠️ Denegada")]
        Denegada = 3,

        [Description("🔄 Aguardando Retorno")]
        AguardandoRetorno = 4
    }
}
