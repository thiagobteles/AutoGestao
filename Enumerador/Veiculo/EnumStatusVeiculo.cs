using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumStatusVeiculo
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Novo")]
        Novo = 1,

        [Description("Usado")]
        Usado = 2
    }
}