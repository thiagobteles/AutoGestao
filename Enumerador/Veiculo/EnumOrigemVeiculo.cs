using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumOrigemVeiculo
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Nacional")]
        Nacional = 1,

        [Description("Importado")]
        Importado = 2
    }
}