using System.ComponentModel;

namespace AutoGestao.Enumerador.Veiculo
{
    public enum EnumEspecieVeiculo
    {
        [Description("Nenhum")]
        Nenhum = 0,

        [Description("Aeronave")]
        Aeronave = 1,

        [Description("Automóvel")]
        Automovel = 2,

        [Description("Bicicleta")]
        Bicicleta = 3,

        [Description("Buggy")]
        Buggy = 4,

        [Description("Caminhão")]
        Caminhao = 5,

        [Description("Carroceria")]
        Carroceria = 6,

        [Description("Ciclomotor")]
        Ciclomotor = 7,

        [Description("Especial / Motor Casa")]
        EspecialMotorCasa = 8,

        [Description("Máquinas")]
        Maquinas = 9,

        [Description("Moto")]
        Moto = 10,

        [Description("Náutica")]
        Nautica = 11,

        [Description("Ônibus")]
        Onibus = 12,

        [Description("Patinete")]
        Patinete = 13,

        [Description("Quadriciclo")]
        Quadriciclo = 14,

        [Description("Reboque")]
        Reboque = 15,

        [Description("Semi-reboque")]
        SemiReboque = 16,

        [Description("Triciclo")]
        Triciclo = 17,

        [Description("UTV")]
        UTV = 18
    }
}