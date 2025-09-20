using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumCambioVeiculo
    {
        [Icone("")]
        [Description("Nenhum")]
        Nenhum = 0,
     
        [Icone("⚙️")]
        [Description("Automático")]
        Automatico = 1,
        
        [Icone("🤖")]
        [Description("Automático sequencial")]
        Automatico_Sequencial = 2,
        
        [Icone("🔄")]
        [Description("CVT")]
        CVT = 3,
        
        [Icone("🎛️")]
        [Description("Manual")]
        Manual = 4,
        
        [Icone("")]
        [Description("Semi-Automático")]
        Semi_Automatico = 5
    }
}