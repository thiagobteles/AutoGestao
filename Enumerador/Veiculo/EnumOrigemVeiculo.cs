using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador.Veiculo
{
    public enum EnumOrigemVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-flag")]
        [Description("Nacional")]
        Nacional = 1,

        [Icone("fas fa-globe-americas")]
        [Description("Importado")]
        Importado = 2
    }
}