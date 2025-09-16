using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumPericiaCautelarVeiculo
    {
        [Description("Nenhum")]
        Nenhuma = 0,

        [Description("Aprovado")]
        Aprovado = 1,

        [Description("Aprovado com observação")]
        Aprovado_Obsevarcao = 2,

        [Description("Aprovado com restrição")]
        Aprovado_Restricao = 3,

        [Description("Reprovado")]
        Reprovado = 4
    }
}