using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumTipoVeiculo
    {
        [Icone("")]
        [Description("Nenhum")]
        Nenhum = 0,
     
        [Icone("🏢")]
        [Description("Próprio")]
        Proprio = 1,
        
        [Icone("🤝")]
        [Description("Consignado")]
        Consignado = 2,

        [Icone("👥")]
        [Description("Terceiros")]
        Terceiros = 3
    }
}