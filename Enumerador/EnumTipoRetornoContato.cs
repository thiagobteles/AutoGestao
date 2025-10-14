using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumTipoRetornoContato
    {
        [Icone("💬")]
        [Description("Whatsapp")]
        Whatsapp = 1,

        [Icone("📞")]
        [Description("Ligação")]
        Ligacao = 2
    }
}