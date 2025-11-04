using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumCambioVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-cog")]
        [Description("Automático")]
        Automatico = 1,

        [Icone("fas fa-robot")]
        [Description("Automático sequencial")]
        Automatico_Sequencial = 2,

        [Icone("fas fa-sync-alt")]
        [Description("CVT")]
        CVT = 3,

        [Icone("fas fa-sliders-h")]
        [Description("Manual")]
        Manual = 4,

        [Icone("fas fa-cogs")]
        [Description("Semi-Automático")]
        Semi_Automatico = 5
    }
}