using AutoGestao.Atributes;
using System.ComponentModel;

namespace AutoGestao.Enumerador
{
    public enum EnumEstado
    {
        [Icone("❓")]
        [Description("Nenhum")]
        Nenhum = 0,

        [Icone("🌳")]
        [Description("AC")]
        Acre = 1,

        [Icone("🌴")]
        [Description("AL")]
        Alagoas = 2,

        [Icone("🌊")]
        [Description("AP")]
        Amapa = 3,

        [Icone("🌲")]
        [Description("AM")]
        Amazonas = 4,

        [Icone("🏖️")]
        [Description("BA")]
        Bahia = 5,

        [Icone("☀️")]
        [Description("CE")]
        Ceara = 6,

        [Icone("🏛️")]
        [Description("DF")]
        DistritoFederal = 7,

        [Icone("🏔️")]
        [Description("ES")]
        EspiritoSanto = 8,

        [Icone("🌾")]
        [Description("GO")]
        Goias = 9,

        [Icone("🌴")]
        [Description("MA")]
        Maranhao = 10,

        [Icone("🐂")]
        [Description("MT")]
        MatoGrosso = 11,

        [Icone("🌿")]
        [Description("MS")]
        MatoGrossoDoSul = 12,

        [Icone("⛰️")]
        [Description("MG")]
        MinasGerais = 13,

        [Icone("🌲")]
        [Description("PA")]
        Para = 14,

        [Icone("🏜️")]
        [Description("PB")]
        Paraiba = 15,

        [Icone("🌲")]
        [Description("PR")]
        Parana = 16,

        [Icone("🏖️")]
        [Description("PE")]
        Pernambuco = 17,

        [Icone("🌵")]
        [Description("PI")]
        Piaui = 18,

        [Icone("🏖️")]
        [Description("RJ")]
        RioDeJaneiro = 19,

        [Icone("🏖️")]
        [Description("RN")]
        RioGrandeDoNorte = 20,

        [Icone("🍇")]
        [Description("RS")]
        RioGrandeDoSul = 21,

        [Icone("🌲")]
        [Description("RO")]
        Rondonia = 22,

        [Icone("🌴")]
        [Description("RR")]
        Roraima = 23,

        [Icone("🌊")]
        [Description("SC")]
        SantaCatarina = 24,

        [Icone("🏙️")]
        [Description("SP")]
        SaoPaulo = 25,

        [Icone("🏖️")]
        [Description("SE")]
        Sergipe = 26,

        [Icone("🌾")]
        [Description("TO")]
        Tocantins = 27
    }
}