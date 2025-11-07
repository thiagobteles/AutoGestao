using System.ComponentModel;

namespace AutoGestao.Enumerador.Fiscal
{
    /// <summary>
    /// Tipo de nota fiscal (Entrada ou Saída)
    /// </summary>
    public enum EnumTipoNotaFiscal
    {
        [Description("📥 Entrada")]
        Entrada = 0,

        [Description("📤 Saída")]
        Saida = 1
    }
}
