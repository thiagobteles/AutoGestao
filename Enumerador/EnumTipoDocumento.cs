using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumTipoDocumento
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("📄")]
        [Description("CRV")]
        CRV = 1,

        [Icone("📋")]
        [Description("CRLV")]
        CRLV = 2,

        [Icone("🧾")]
        [Description("Nota fiscal")]
        NotaFiscal = 3
    }
}