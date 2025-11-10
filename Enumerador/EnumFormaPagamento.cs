// Enumerador/EnumFormaPagamento.cs
using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumFormaPagamento
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-money-bill-wave")]
        [Description("Dinheiro")]
        Dinheiro = 1,

        [Icone("fas fa-mobile-alt")]
        [Description("PIX")]
        Pix = 2,

        [Icone("fas fa-credit-card")]
        [Description("Cartão de Crédito")]
        CartaoCredito = 3,

        [Icone("fas fa-credit-card")]
        [Description("Cartão de Débito")]
        CartaoDebito = 4,

        [Icone("fas fa-file-invoice")]
        [Description("Boleto Bancário")]
        Boleto = 5,

        [Icone("fas fa-university")]
        [Description("Transferência Bancária")]
        TransferenciaBancaria = 6,

        [Icone("fas fa-file-signature")]
        [Description("Cheque")]
        Cheque = 7,

        [Icone("fas fa-hand-holding-usd")]
        [Description("Financiamento")]
        Financiamento = 8,

        [Icone("fas fa-bullseye")]
        [Description("Consórcio")]
        Consorcio = 9,

        [Icone("fas fa-piggy-bank")]
        [Description("Depósito Bancário")]
        DepositoBancario = 10,

        [Icone("fas fa-clipboard-list")]
        [Description("Crediário")]
        Crediario = 11,

        [Icone("fas fa-exchange-alt")]
        [Description("Permuta/Troca")]
        Permuta = 12,

        [Icone("fas fa-coins")]
        [Description("Dinheiro + Cartão")]
        DinheiroCartao = 13,

        [Icone("fas fa-wallet")]
        [Description("PIX + Cartão")]
        PixCartao = 14,

        [Icone("fas fa-money-check-alt")]
        [Description("Dinheiro + PIX")]
        DinheiroPix = 15,

        [Icone("fas fa-random")]
        [Description("Misto")]
        Misto = 16
    }
}