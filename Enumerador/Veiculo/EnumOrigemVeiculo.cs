using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumOrigemVeiculo
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("🇧🇷")]
        [Description("Nacional")]
        Nacional = 1,

        [Icone("🌎")]
        [Description("Importado")]
        Importado = 2
    }
}