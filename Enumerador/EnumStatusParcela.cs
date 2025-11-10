using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumStatusParcela
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-hourglass-half")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("fas fa-check-circle")]
        [Description("Paga")]
        Paga = 2,

        [Icone("fas fa-exclamation-triangle")]
        [Description("Vencida")]
        Vencida = 3
    }
}