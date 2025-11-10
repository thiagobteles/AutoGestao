using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumStatusAvaliacao
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-hourglass-half")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("fas fa-check-circle")]
        [Description("Aprovada")]
        Aprovada = 2,

        [Icone("fas fa-times-circle")]
        [Description("Rejeitada")]
        Rejeitada = 3
    }
}