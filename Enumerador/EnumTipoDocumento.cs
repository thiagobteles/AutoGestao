using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumTipoDocumento
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("CRV")]
        CRV = 1,

        [Description("CRLV")]
        CRLV = 2,

        [Description("Nota fiscal")]
        NotaFiscal = 3
    }
}