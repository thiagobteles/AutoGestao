namespace AutoGestao.Extensions
{
    /// <summary>
    /// Configurações de convenção para o projeto
    /// </summary>
    public static class ProjectConventions
    {
        /// <summary>
        /// Convenção do projeto: Controllers no SINGULAR
        /// Exemplos: Veiculo, Cliente, Fornecedor (não Veiculos, Clientes, Fornecedores)
        /// </summary>
        public static string GetControllerName<T>() where T : class
        {
            return typeof(T).Name; // Usar nome da entidade no SINGULAR
        }

        /// <summary>
        /// Gera URLs baseadas na convenção SINGULAR
        /// </summary>
        public static string GetControllerUrl<T>(string action, long? id = null) where T : class
        {
            var controllerName = GetControllerName<T>();

            return id.HasValue
                ? $"/{controllerName}/{action}/{id}"
                : $"/{controllerName}/{action}";
        }

        /// <summary>
        /// Gera títulos baseados na convenção SINGULAR
        /// </summary>
        public static string GetControllerTitle<T>() where T : class
        {
            return GetControllerName<T>(); // Manter singular
        }
    }
}
