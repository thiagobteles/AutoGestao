using System.ComponentModel;

namespace FGT.Enumerador.Fiscal
{
    public enum EnumTipoContaContabil
    {
        [Description("💰 Ativo")]
        Ativo = 1,

        [Description("📊 Passivo")]
        Passivo = 2,

        [Description("💼 Patrimônio Líquido")]
        PatrimonioLiquido = 3,

        [Description("📈 Receita")]
        Receita = 4,

        [Description("📉 Despesa")]
        Despesa = 5,

        [Description("💵 Custos")]
        Custos = 6
    }
}
