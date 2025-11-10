using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador.Veiculo
{
    public enum EnumStatusVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-certificate")]
        [Description("Novo")]
        Novo = 1,

        [Icone("fas fa-car")]
        [Description("Usado")]
        Usado = 2
    }
}