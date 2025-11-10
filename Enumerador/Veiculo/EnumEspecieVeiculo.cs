using FGT.Atributes;
using System.ComponentModel;

namespace FGT.Enumerador.Veiculo
{
    public enum EnumEspecieVeiculo
    {
        [Icone("fas fa-question-circle")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("fas fa-plane")]
        [Description("Aeronave")]
        Aeronave = 1,

        [Icone("fas fa-car")]
        [Description("Automóvel")]
        Automovel = 2,

        [Icone("fas fa-bicycle")]
        [Description("Bicicleta")]
        Bicicleta = 3,

        [Icone("fas fa-car-side")]
        [Description("Buggy")]
        Buggy = 4,

        [Icone("fas fa-truck")]
        [Description("Caminhão")]
        Caminhao = 5,

        [Icone("fas fa-box")]
        [Description("Carroceria")]
        Carroceria = 6,

        [Icone("fas fa-motorcycle")]
        [Description("Ciclomotor")]
        Ciclomotor = 7,

        [Icone("fas fa-shuttle-van")]
        [Description("Especial / Motor Casa")]
        EspecialMotorCasa = 8,

        [Icone("fas fa-tractor")]
        [Description("Máquinas")]
        Maquinas = 9,

        [Icone("fas fa-motorcycle")]
        [Description("Moto")]
        Moto = 10,

        [Icone("fas fa-ship")]
        [Description("Náutica")]
        Nautica = 11,

        [Icone("fas fa-bus")]
        [Description("Ônibus")]
        Onibus = 12,

        [Icone("fas fa-skating")]
        [Description("Patinete")]
        Patinete = 13,

        [Icone("fas fa-flag-checkered")]
        [Description("Quadriciclo")]
        Quadriciclo = 14,

        [Icone("fas fa-link")]
        [Description("Reboque")]
        Reboque = 15,

        [Icone("fas fa-link")]
        [Description("Semi-reboque")]
        SemiReboque = 16,

        [Icone("fas fa-motorcycle")]
        [Description("Triciclo")]
        Triciclo = 17,

        [Icone("fas fa-mountain")]
        [Description("UTV")]
        UTV = 18
    }
}