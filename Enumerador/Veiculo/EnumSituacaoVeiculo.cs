using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumSituacaoVeiculo
    {
        [Icone("")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("📦")]
        [Description("Estoque")]
        Estoque = 1,

        [Icone("✅")]
        [Description("Vendido")]
        Vendido = 2,

        [Icone("🔒")]
        [Description("Reservado")]
        Reservado = 3,

        [Icone("🔧")]
        [Description("Manutenção")]
        Manutencao = 4,

        [Icone("🔄")]
        [Description("Transferido")]
        Transferido = 5
    }
}