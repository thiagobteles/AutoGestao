using FGT.Enumerador.Gerais;

namespace FGT
{
    public static class Globais
    {
        public static string Cliente { get; set; }

        public static EnumCorSistema CorSistema { get; set; }

        public static string NomeApresentacao
        {
            get
            {
                return EhSistemaVeiculo
                    ? "Auto Gestão" 
                        : EhSistemaNotaFiscal
                        ? "Nota Goiana"
                        : "Negociação fácil";
            }
        }

        public static string Icone
        {
            get
            {
                return EhSistemaVeiculo
                    ? "fas fa-car"
                        : EhSistemaNotaFiscal
                        ? "fas fa-graduation-cap"
                        : "fas fa-calculator";
            }
        }

        public static bool EhSistemaVeiculo
        {
            get
            {
                return Cliente.Equals("autogestao", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public static bool EhSistemaNotaFiscal
        {
            get
            {
                return Cliente.Equals("contabilidade", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public static bool EhSistemaParcelaFacil
        {
            get
            {
                return Cliente.Equals("parcelafacil", StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }
}
