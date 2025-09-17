using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumPrioridade
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Baixa")]
        Baixa = 1,

        [Description("Média")]
        Media = 2,

        [Description("Alta")]
        Alta = 3,

        [Description("Crítica")]
        Critica = 4
    }
}