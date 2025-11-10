using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador
{
    public enum EnumTipoPessoa
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-user")]
        [Description("Pessoa física")]
        PessoaFisica = 1,

        [Icone("fas fa-building")]
        [Description("Pessoa Jurídica")]
        PessoaJuridica = 2
    }
}