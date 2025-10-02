// Enumerador/EnumFormaPagamento.cs
using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumFormaPagamento
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("💵")]
        [Description("Dinheiro")]
        Dinheiro = 1,

        [Icone("📱")]
        [Description("PIX")]
        Pix = 2,

        [Icone("💳")]
        [Description("Cartão de Crédito")]
        CartaoCredito = 3,

        [Icone("💳")]
        [Description("Cartão de Débito")]
        CartaoDebito = 4,

        [Icone("📄")]
        [Description("Boleto Bancário")]
        Boleto = 5,

        [Icone("🏦")]
        [Description("Transferência Bancária")]
        TransferenciaBancaria = 6,

        [Icone("📝")]
        [Description("Cheque")]
        Cheque = 7,

        [Icone("🏦")]
        [Description("Financiamento")]
        Financiamento = 8,

        [Icone("🎯")]
        [Description("Consórcio")]
        Consorcio = 9,

        [Icone("💰")]
        [Description("Depósito Bancário")]
        DepositoBancario = 10,

        [Icone("📋")]
        [Description("Crediário")]
        Crediario = 11,

        [Icone("🔄")]
        [Description("Permuta/Troca")]
        Permuta = 12,

        [Icone("💵💳")]
        [Description("Dinheiro + Cartão")]
        DinheiroCartao = 13,

        [Icone("📱💳")]
        [Description("PIX + Cartão")]
        PixCartao = 14,

        [Icone("💵📱")]
        [Description("Dinheiro + PIX")]
        DinheiroPix = 15,

        [Icone("🔀")]
        [Description("Misto")]
        Misto = 16
    }
}