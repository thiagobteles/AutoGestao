using System.ComponentModel;

namespace FGT.Enumerador.Fiscal
{
    public enum EnumTipoConta
    {
        [Description("💰 Conta Corrente")]
        ContaCorrente = 1,

        [Description("💎 Conta Poupança")]
        ContaPoupanca = 2,

        [Description("💳 Conta Investimento")]
        ContaInvestimento = 3,

        [Description("🏦 Conta Salário")]
        ContaSalario = 4,

        [Description("📱 Conta Digital")]
        ContaDigital = 5,

        [Description("🌐 Conta Internacional")]
        ContaInternacional = 6
    }
}
