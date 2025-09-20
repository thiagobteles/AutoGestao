using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumCombustivelVeiculo
    {
        [Icone("❓")]
        [Description("Nenhum")] 
        Nenhum = 0,

        [Icone("🌽")]
        [Description("Etanol")]
        Etanol = 1,

        [Icone("")]
        [Description("AVGAS")]
        Avgas = 2,

        [Icone("🚛")]
        [Description("Diesel")]
        Diesel = 3,

        [Icone("🔋")]
        [Description("Elétrico")]
        Eletrico = 4,

        [Icone("🔀")]
        [Description("Flex")]
        Flex = 5,

        [Icone("⛽")]
        [Description("Gasolina")]
        Gasolina = 6,

        [Icone("")]
        [Description("GNV")]
        Gnv = 7,

        [Icone("🔋⛽")]
        [Description("Híbrido")]
        Hibrido = 8,

        [Icone("")]
        [Description("JET A-1")]
        JET_A1 = 9,

        [Icone("❓")]
        [Description("Outros")]
        Outros = 10
    }
}