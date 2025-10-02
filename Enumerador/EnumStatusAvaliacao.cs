using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusAvaliacao
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("⏳")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("✅")]
        [Description("Aprovada")]
        Aprovada = 2,

        [Icone("❌")]
        [Description("Rejeitada")]
        Rejeitada = 3
    }
}