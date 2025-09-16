using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumCombustivelVeiculo
    {
        [Description("Nenhum")] 
        Nenhum = 0,
        
        [Description("Alcool")]
        Alcool = 1,

        [Description("AVGAS")]
        Avgas = 2,

        [Description("Diesel")]
        Diesel = 3,

        [Description("Elétrico")]
        Eletrico = 4,

        [Description("Flex")]
        Flex = 5,

        [Description("Gasolina")]
        Gasolina = 6,

        [Description("GNV")]
        Gnv = 7,

        [Description("Híbrido")]
        Hibrido = 8,

        [Description("JET A-1")]
        JET_A1 = 9,

        [Description("Outros")]
        Outros = 10
    }
}