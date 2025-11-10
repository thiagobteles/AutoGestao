using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumTipoRetornoContato
    {
        [Icone("fab fa-whatsapp")]
        [Description("Whatsapp")]
        Whatsapp = 1,

        [Icone("fas fa-phone")]
        [Description("Ligação")]
        Ligacao = 2
    }
}