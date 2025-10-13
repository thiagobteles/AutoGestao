using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumTipoContato
    {
        [Icone("")]
        [Description("Whatsapp")]
        Whatsapp = 1,

        [Icone("")]
        [Description("Ligação")]
        Ligacao = 2
    }
}