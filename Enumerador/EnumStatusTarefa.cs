using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusTarefa
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Pendente")]
        Pendente = 1,

        [Description("Em Andamento")]
        EmAndamento = 2,

        [Description("Concluida")]
        Concluida = 3,

        [Description("Cancelada")]
        Cancelada = 4
    }
}