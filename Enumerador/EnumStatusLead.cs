using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumStatusLead
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-hourglass-half")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("fas fa-check-circle")]
        [Description("Executado")]
        Executado = 2,

        [Icone("fas fa-times-circle")]
        [Description("Cancelado")]
        Cancelado = 3,

        [Icone("fas fa-money-bill-wave")]
        [Description("Encaminhado financeiro")]
        EncaminhadoParaFinanceiro = 4
    }
}