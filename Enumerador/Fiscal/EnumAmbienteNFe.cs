using System.ComponentModel;

namespace AutoGestao.Enumerador.Fiscal
{
    public enum EnumAmbienteNFe
    {
        [Description("🔧 Homologação (Teste)")]
        Homologacao = 2,

        [Description("✅ Produção (Real)")]
        Producao = 1
    }
}
