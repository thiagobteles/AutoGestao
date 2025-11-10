using System.ComponentModel;

namespace FGT.Enumerador.Fiscal
{
    public enum EnumTipoObrigacaoFiscal
    {
        [Description("📊 SPED Fiscal")]
        SPEDFiscal = 1,

        [Description("💼 SPED Contribuições")]
        SPEDContribuicoes = 2,

        [Description("📈 SPED Contábil (ECD)")]
        SPEDContabil = 3,

        [Description("💰 DCTF")]
        DCTF = 4,

        [Description("🔵 DCTFWeb")]
        DCTFWeb = 5,

        [Description("👥 eSocial")]
        ESocial = 6,

        [Description("📄 DIRF")]
        DIRF = 7,

        [Description("💵 DARF")]
        DARF = 8,

        [Description("🟢 DAS (Simples Nacional)")]
        DAS = 9,

        [Description("📑 GFIP")]
        GFIP = 10,

        [Description("📋 DEFIS")]
        DEFIS = 11,

        [Description("💼 DeSTDA")]
        DeSTDA = 12,

        [Description("📊 GIA")]
        GIA = 13,

        [Description("📄 DIME")]
        DIME = 14,

        [Description("🏛️ ISS (Declaração Municipal)")]
        ISS = 15,

        [Description("📈 EFD-Reinf")]
        EFDReinf = 16
    }
}
