using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumTipoPessoa
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Pessoa física")]
        PessoaFisica = 1,

        [Description("Pessoa Jurídica")]
        PessoaJuridica = 2
    }
}