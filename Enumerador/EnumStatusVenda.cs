using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusVenda
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("⏳")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("✅")]
        [Description("Concluida")]
        Concluida = 2,

        [Icone("❌")]
        [Description("Cancelada")]
        Cancelada = 3
    }
}