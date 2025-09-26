using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumTipoPessoa
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("👤")]
        [Description("Pessoa física")]
        PessoaFisica = 1,

        [Icone("🏢")]
        [Description("Pessoa Jurídica")]
        PessoaJuridica = 2
    }
}