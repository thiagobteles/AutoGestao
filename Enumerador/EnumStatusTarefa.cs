using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusTarefa
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("⏳")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("🔄")]
        [Description("Em Andamento")]
        EmAndamento = 2,

        [Icone("✅")]
        [Description("Concluida")]
        Concluida = 3,

        [Icone("❌")]
        [Description("Cancelada")]
        Cancelada = 4
    }
}