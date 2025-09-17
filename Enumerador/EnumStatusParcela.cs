using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusParcela
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Pendente")]
        Pendente = 1,

        [Description("Paga")]
        Paga = 2,

        [Description("Vencida")]
        Vencida = 3
    }
}