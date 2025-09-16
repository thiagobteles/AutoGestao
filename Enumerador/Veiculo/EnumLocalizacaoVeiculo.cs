using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumLocalizacaoVeiculo
    {
        [Description("Nenhum")]
        Nenhum = 0,
     
        [Description("Pátio")]
        Patio = 1,
        
        [Description("Cliente")]
        Cliente = 2
    }
}