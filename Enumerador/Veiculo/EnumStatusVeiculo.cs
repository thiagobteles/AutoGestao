using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumStatusVeiculo
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("🆕")]
        [Description("Novo")]
        Novo = 1,

        [Icone("✨")]
        [Description("Usado")]
        Usado = 2
    }
}