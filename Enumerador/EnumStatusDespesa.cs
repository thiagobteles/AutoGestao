using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusDespesa
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("⏳")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("✅")]
        [Description("Pago")]
        Pago = 2,

        [Icone("❌")]
        [Description("Cancelado")]
        Cancelado = 3
    }
}