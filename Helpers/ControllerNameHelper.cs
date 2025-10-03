namespace AutoGestao.Helpers
{
    /// <summary>
    /// Helper genérico para resolver nomes de controllers baseado em convenção
    /// Convenção: NomeEntidade + "s" = NomeController
    /// Exemplo: Cliente -> Clientes, Veiculo -> Veiculos, VeiculoMarca -> VeiculoMarcas
    /// </summary>
    public static class ControllerNameHelper
    {
        /// <summary>
        /// Obtém o nome do controller baseado no nome da entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade (ex: "Cliente", "Veiculo")</param>
        /// <returns>Nome do controller (ex: "Clientes", "Veiculos")</returns>
        public static string GetControllerName(string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName))
            {
                throw new ArgumentException("Nome da entidade não pode ser vazio", nameof(entityName));
            }

            // Convenção simples: Nome da entidade + "s"
            return entityName + "s";
        }

        /// <summary>
        /// Obtém o nome do controller baseado no tipo da entidade
        /// </summary>
        /// <param name="entityType">Tipo da entidade</param>
        /// <returns>Nome do controller</returns>
        public static string GetControllerName(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            return GetControllerName(entityType.Name);
        }

        /// <summary>
        /// Obtém a URL completa para uma action de um controller
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="action">Action do controller (ex: "Index", "Details", "Edit")</param>
        /// <param name="id">ID opcional do registro</param>
        /// <returns>URL completa (ex: "/Clientes/Details/123")</returns>
        public static string GetActionUrl(string entityName, string action, long? id = null)
        {
            var controller = GetControllerName(entityName);
            var url = $"/{controller}/{action}";

            if (id.HasValue)
            {
                url += $"/{id}";
            }

            return url;
        }

        /// <summary>
        /// Obtém a URL de detalhes de uma entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="id">ID do registro</param>
        /// <returns>URL de detalhes (ex: "/Clientes/Details/123")</returns>
        public static string GetDetailsUrl(string entityName, long id)
        {
            return GetActionUrl(entityName, "Details", id);
        }

        /// <summary>
        /// Obtém a URL de edição de uma entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="id">ID do registro</param>
        /// <returns>URL de edição (ex: "/Clientes/Edit/123")</returns>
        public static string GetEditUrl(string entityName, long id)
        {
            return GetActionUrl(entityName, "Edit", id);
        }

        /// <summary>
        /// Obtém a URL de criação de uma entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <returns>URL de criação (ex: "/Clientes/Create")</returns>
        public static string GetCreateUrl(string entityName)
        {
            return GetActionUrl(entityName, "Create");
        }
    }
}