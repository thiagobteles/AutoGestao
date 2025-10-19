using AutoGestao.Helpers;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumSituacaoVeiculo
    {
        [Description("Disponível para venda")]
        [Icon("fas fa-check-circle")]
        [CssClass("text-success")]
        Estoque = 1,

        [Description("Vendido")]
        [Icon("fas fa-handshake")]
        [CssClass("text-info")]
        Vendido = 2,

        [Description("Em manutenção")]
        [Icon("fas fa-wrench")]
        [CssClass("text-warning")]
        EmManutencao = 3,

        [Description("Reservado")]
        [Icon("fas fa-bookmark")]
        [CssClass("text-primary")]
        Reservado = 4
    }
}