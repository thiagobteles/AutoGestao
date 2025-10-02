using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusParcela
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("⏳")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("✅")]
        [Description("Paga")]
        Paga = 2,

        [Icone("⚠️")]
        [Description("Vencida")]
        Vencida = 3
    }
}