using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumTipoVeiculo
    {
        [Description("Nenhum")]
        Nenhum = 0,
     
        [Description("Próprio")]
        Proprio = 1,
        
        [Description("Consignado")]
        Consignado = 2
    }
}