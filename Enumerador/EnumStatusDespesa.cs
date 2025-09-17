using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusDespesa
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Pendente")]
        Pendente = 1,

        [Description("Pago")]
        Pago = 2,

        [Description("Cancelado")]
        Cancelado = 3
    }
}