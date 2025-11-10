using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumTipoDocumento
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-file-alt")]
        [Description("CRV")]
        CRV = 1,

        [Icone("fas fa-clipboard")]
        [Description("CRLV")]
        CRLV = 2,

        [Icone("fas fa-receipt")]
        [Description("Nota fiscal")]
        NotaFiscal = 3
    }
}