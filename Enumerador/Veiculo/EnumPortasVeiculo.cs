using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumPortasVeiculo
    {
        [Icone("⭕")]
        [Description("0")]
        Nenhuma = 0,

        [Icone("1️⃣")]
        [Description("1")]
        Uma = 1,

        [Icone("2️⃣")]
        [Description("2")]
        Duas = 2,

        [Icone("3️⃣")]
        [Description("3")]
        Tres = 3,

        [Icone("4️⃣")]
        [Description("4")]
        Quatro = 4,

        [Icone("5️⃣")]
        [Description("5")]
        Cinco = 5,

        [Icone("6️⃣")]
        [Description("6")]
        Seis = 6,
    }
}