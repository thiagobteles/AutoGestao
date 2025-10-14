using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumStatusLead
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("⏳")]
        [Description("Pendente")]
        Pendente = 1,

        [Icone("✅")]
        [Description("Executado")]
        Executado = 2,

        [Icone("❌")]
        [Description("Cancelado")]
        Cancelado = 3,

        [Icone("💰")]
        [Description("Encaminhado financeiro")]
        EncaminhadoParaFinanceiro = 4
    }
}