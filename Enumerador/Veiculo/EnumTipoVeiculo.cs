using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumTipoVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-warehouse")]
        [Description("Próprio")]
        Proprio = 1,

        [Icone("fas fa-handshake")]
        [Description("Consignado")]
        Consignado = 2,

        [Icone("fas fa-users")]
        [Description("Terceiros")]
        Terceiros = 3
    }
}