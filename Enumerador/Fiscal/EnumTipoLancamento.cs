using System.ComponentModel;

namespace AutoGestao.Enumerador.Fiscal
{
    public enum EnumTipoLancamento
    {
        [Description("📝 Manual")]
        Manual = 1,

        [Description("🤖 Automático")]
        Automatico = 2,

        [Description("🔄 Importado")]
        Importado = 3,

        [Description("📊 Integração")]
        Integracao = 4,

        [Description("⚙️ Sistema")]
        Sistema = 5
    }
}
