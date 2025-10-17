namespace AutoGestao
{
    public static class Globais
    {
        public static string Cliente { get; set; }

        public static string NomeApresentacao 
        {
            get
            {
                return EhAutoGestao ? "Auto Gestão" : "Instituto FD";
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
