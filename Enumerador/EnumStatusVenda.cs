using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusVenda
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Pendente")]
        Pendente = 1,

        [Description("Concluida")]
        Concluida = 2,

        [Description("Cancelada")]
        Cancelada = 3
    }
}