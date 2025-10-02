using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumPrioridade
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("🟢")]
        [Description("Baixa")]
        Baixa = 1,

        [Icone("🟡")]
        [Description("Média")]
        Media = 2,

        [Icone("🟠")]
        [Description("Alta")]
        Alta = 3,

        [Icone("🔴")]
        [Description("Crítica")]
        Critica = 4
    }
}