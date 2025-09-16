using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumCarroceriaVeiculo
    {
        [Description("Nenhum")] 
        Nenhum = 0,

        [Description("Camionete")] 
        Camionete = 1,
        
        [Description("Conversivel")]
        Conversivel = 2,

        [Description("Coupe")]
        Coupe = 3,

        [Description("Furgão")]
        Furgao = 4,

        [Description("Hatch")]
        Hatch = 5,

        [Description("Minivan")]
        Minivan = 6,

        [Description("Off-Road")] 
        Off_Road = 7,

        [Description("Sedan")] 
        Sedan = 8,

        [Description("Station Wagon")] 
        Station_Wagon = 9,

        [Description("SUV")]
        SUV = 10,

        [Description("Utilitário")]
        Utilitario = 11
    }
}