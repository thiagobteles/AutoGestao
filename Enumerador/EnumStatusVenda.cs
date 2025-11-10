using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumStatusVenda
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-hourglass-half")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("fas fa-check-circle")]
        [Description("Concluida")]
        Concluida = 2,

        [Icone("fas fa-times-circle")]
        [Description("Cancelada")]
        Cancelada = 3
    }
}