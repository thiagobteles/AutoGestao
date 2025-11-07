using System.ComponentModel;

namespace AutoGestao.Enumerador.Fiscal
{
    public enum EnumStatusObrigacao
    {
        [Description("⏳ Pendente")]
        Pendente = 1,

        [Description("⚙️ Em Andamento")]
        EmAndamento = 2,

        [Description("✅ Entregue")]
        Entregue = 3,

        [Description("⚠️ Atrasada")]
        Atrasada = 4,

        [Description("❌ Retificada")]
        Retificada = 5,

        [Description("🔄 Aguardando Processamento")]
        AguardandoProcessamento = 6,

        [Description("❗ Rejeitada")]
        Rejeitada = 7,

        [Description("🚫 Dispensada")]
        Dispensada = 8
    }
}
