using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumCombustivelVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-leaf")]
        [Description("Etanol")]
        Etanol = 1,

        [Icone("fas fa-plane")]
        [Description("AVGAS")]
        Avgas = 2,

        [Icone("fas fa-truck")]
        [Description("Diesel")]
        Diesel = 3,

        [Icone("fas fa-battery-full")]
        [Description("Elétrico")]
        Eletrico = 4,

        [Icone("fas fa-random")]
        [Description("Flex")]
        Flex = 5,

        [Icone("fas fa-gas-pump")]
        [Description("Gasolina")]
        Gasolina = 6,

        [Icone("fas fa-wind")]
        [Description("GNV")]
        Gnv = 7,

        [Icone("fas fa-charging-station")]
        [Description("Híbrido")]
        Hibrido = 8,

        [Icone("fas fa-plane")]
        [Description("JET A-1")]
        JET_A1 = 9,

        [Icone("fas fa-question-circle")]
        [Description("Outros")]
        Outros = 10
    }
}