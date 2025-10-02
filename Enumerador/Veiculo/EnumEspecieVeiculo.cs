using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumEspecieVeiculo
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("✈️")]
        [Description("Aeronave")]
        Aeronave = 1,

        [Icone("🚗")]
        [Description("Automóvel")]
        Automovel = 2,

        [Icone("🚴")]
        [Description("Bicicleta")]
        Bicicleta = 3,

        [Icone("🏎️")]
        [Description("Buggy")]
        Buggy = 4,

        [Icone("🚛")]
        [Description("Caminhão")]
        Caminhao = 5,

        [Icone("📦")]
        [Description("Carroceria")]
        Carroceria = 6,

        [Icone("🛵")]
        [Description("Ciclomotor")]
        Ciclomotor = 7,

        [Icone("🚐")]
        [Description("Especial / Motor Casa")]
        EspecialMotorCasa = 8,

        [Icone("🚜")]
        [Description("Máquinas")]
        Maquinas = 9,

        [Icone("🏍️")]
        [Description("Moto")]
        Moto = 10,

        [Icone("⛵")]
        [Description("Náutica")]
        Nautica = 11,

        [Icone("🚌")]
        [Description("Ônibus")]
        Onibus = 12,

        [Icone("🛴")]
        [Description("Patinete")]
        Patinete = 13,

        [Icone("🏁")]
        [Description("Quadriciclo")]
        Quadriciclo = 14,

        [Icone("🔗")]
        [Description("Reboque")]
        Reboque = 15,

        [Icone("🔗")]
        [Description("Semi-reboque")]
        SemiReboque = 16,

        [Icone("🛺")]
        [Description("Triciclo")]
        Triciclo = 17,

        [Icone("🏞️")]
        [Description("UTV")]
        UTV = 18
    }
}