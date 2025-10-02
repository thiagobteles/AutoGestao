using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumPericiaCautelarVeiculo
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhuma = 0,

        [Icone("✅")]
        [Description("Aprovado")]
        Aprovado = 1,

        [Icone("⚠️")]
        [Description("Aprovado com observação")]
        Aprovado_Obsevarcao = 2,

        [Icone("🔶")]
        [Description("Aprovado com restrição")]
        Aprovado_Restricao = 3,

        [Icone("❌")]
        [Description("Reprovado")]
        Reprovado = 4
    }
}