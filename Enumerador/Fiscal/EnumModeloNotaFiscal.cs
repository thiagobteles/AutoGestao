using System.ComponentModel;

namespace AutoGestao.Enumerador.Fiscal
{
    public enum EnumModeloNotaFiscal
    {
        [Description("📄 NF-e (Nota Fiscal Eletrônica)")]
        NFe = 55,

        [Description("📱 NFC-e (Cupom Fiscal Eletrônico)")]
        NFCe = 65,

        [Description("🏢 NFS-e (Nota Fiscal de Serviço)")]
        NFSe = 99
    }
}
