using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusAvaliacao
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Pendente")]
        Pendente = 1,

        [Description("Aprovada")]
        Aprovada = 2,

        [Description("Rejeitada")]
        Rejeitada = 3
    }
}