using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumTipoPessoa
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Category("👤")]
        [Description("Pessoa física")]
        PessoaFisica = 1,

        [Category("🏢")]
        [Description("Pessoa Jurídica")]
        PessoaJuridica = 2
    }
}