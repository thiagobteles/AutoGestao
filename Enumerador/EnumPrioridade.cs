using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumPrioridade
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-arrow-down")]
        [Description("Baixa")]
        Baixa = 1,

        [Icone("fas fa-minus")]
        [Description("Média")]
        Media = 2,

        [Icone("fas fa-arrow-up")]
        [Description("Alta")]
        Alta = 3,

        [Icone("fas fa-exclamation-circle")]
        [Description("Crítica")]
        Critica = 4
    }
}