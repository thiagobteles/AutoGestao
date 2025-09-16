using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumSituacaoVeiculo
    {
        [Description("Nenhum")]
        Nenhum = 0,
        
        [Description("Estoque")]
        Estoque = 1,
        
        [Description("Vendido")]
        Vendido = 2,
        
        [Description("Reservado")]
        Reservado = 3,
        
        [Description("Manutenção")]
        Manutencao = 4
    }
}