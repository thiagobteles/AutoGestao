namespace AutoGestao
{
    public static class Globais
    {
        public static string Cliente { get; set; }

        public static string NomeApresentacao 
        {
            get
            {
                return EhAutoGestao ? "Auto Gestão" : "Contabilidade";
            } 
        }

        public static string Icone
        {
            get
            {
                return EhAutoGestao ? "fas fa-car" : "fas fa-graduation-cap";
            }
        }

        public static bool EhAutoGestao
        {
            get
            {
                return Cliente.Equals("autogestao", StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }
}
