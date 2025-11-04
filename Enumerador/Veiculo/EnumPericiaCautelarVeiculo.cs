using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumPericiaCautelarVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhuma = 0,

        [Icone("fas fa-check-circle")]
        [Description("Aprovado")]
        Aprovado = 1,

        [Icone("fas fa-exclamation-triangle")]
        [Description("Aprovado com observação")]
        Aprovado_Obsevarcao = 2,

        [Icone("fas fa-exclamation-circle")]
        [Description("Aprovado com restrição")]
        Aprovado_Restricao = 3,

        [Icone("fas fa-times-circle")]
        [Description("Reprovado")]
        Reprovado = 4
    }
}