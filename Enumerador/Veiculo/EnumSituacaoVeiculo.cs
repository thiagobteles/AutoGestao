using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador.Veiculo
{
    public enum EnumSituacaoVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-box")]
        [Description("Estoque")]
        Estoque = 1,

        [Icone("fas fa-check-circle")]
        [Description("Vendido")]
        Vendido = 2,

        [Icone("fas fa-lock")]
        [Description("Reservado")]
        Reservado = 3,

        [Icone("fas fa-wrench")]
        [Description("Manutenção")]
        Manutencao = 4,

        [Icone("fas fa-exchange-alt")]
        [Description("Transferido")]
        Transferido = 5
    }
}