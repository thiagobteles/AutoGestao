using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumFormaPagamento
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Concluida")]
        Concluida = 1,

        [Description("Cancelada")]
        Cancelada = 2
    }
}