using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumCambioVeiculo
    {
        [Description("Nenhum")]
        Nenhum = 0,
     
        [Description("Automático")]
        Automatico = 1,
        
        [Description("Automático sequencial")]
        Automatico_Sequencial = 2,
        
        [Description("CVT")]
        CVT = 3,
        
        [Description("Manual")]
        Manual = 4,
        
        [Description("Semi-Automático")]
        Semi_Automatico = 5
    }
}