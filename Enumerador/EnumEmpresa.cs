using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumEmpresa
    {
        [Description("Nenhum")]
        Nenhum = 0,
     
        [Description("Matriz")]
        Matriz = 1,
        
        [Description("Filial")]
        Filial = 2
    }
}