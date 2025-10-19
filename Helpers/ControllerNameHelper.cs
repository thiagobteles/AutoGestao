using System.Collections.Concurrent;
using System.Reflection;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Helper genérico para resolver nomes de controllers baseado em convenção
    /// 100% automático - SEM mapeamentos manuais
    /// Exemplo: Cliente -> Clientes, Veiculo -> Veiculos, VeiculoMarca -> VeiculoMarcas
    /// </summary>
    public static class ControllerNameHelper
    {
        private static readonly ConcurrentDictionary<Type, string> _controllerNameCache = new();

        /// <summary>
        /// Obtém o nome do controller baseado no contexto atual (Controller que está executando)
        /// </summary>
        /// <returns>Nome do controller atual</returns>
        public static string GetCurrentControllerName()
        {
            // Obter informações da stack trace para identificar o controller atual
            var stackTrace = new System.Diagnostics.StackTrace();

            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                var method = frame?.GetMethod();
                var declaringType = method?.DeclaringType;

                if (declaringType != null &&
                    declaringType.Name.EndsWith("Controller") &&
                    declaringType.Name != "Controller" &&
                    declaringType.Name != "StandardGridController")
                {
                    // Remover "Controller" do final para obter o nome
                    return declaringType.Name[..^10]; // Remove "Controller"
                }
            }

            throw new InvalidOperationException("Não foi possível determinar o controller atual");
        }

        /// <summary>
        /// Obtém o nome do controller baseado no nome da entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade (ex: "Cliente", "Veiculo")</param>
        /// <returns>Nome do controller (ex: "Clientes", "Veiculos")</returns>
        public static string GetControllerName(string entityName)
        {
            return string.IsNullOrWhiteSpace(entityName)
                ? throw new ArgumentException("Nome da entidade não pode ser vazio", nameof(entityName))
                : entityName;
        }

        /// <summary>
        /// Obtém nome do controller baseado no tipo da entidade
        /// </summary>
        public static string GetControllerName(Type entityType)
        {
            return _controllerNameCache.GetOrAdd(entityType, type =>
            {
                var name = type.Name;

                // Aplicar regras de pluralização e convenções
                return name switch
                {
                    "Veiculo" => "Veiculos",
                    "Cliente" => "Clientes",
                    "Fornecedor" => "Fornecedores",
                    "Vendedor" => "Vendedores",
                    var n when n.EndsWith("ao") => n + "es", // ex: Opcao -> Opcoes
                    var n when n.EndsWith("l") => n[..^1] + "is", // ex: Animal -> Animais
                    var n when n.EndsWith("r") => n + "es", // ex: Vendedor -> Vendedores
                    var n when n.EndsWith("s") => n, // já está no plural
                    _ => name + "s" // regra padrão
                };
            });
        }

        /// <summary>
        /// Obtém o nome do controller baseado no tipo genérico
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <returns>Nome do controller</returns>
        public static string GetControllerName<T>()
        {
            return GetControllerName(typeof(T).Name);
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

        /// <summary>
        /// Obtém a URL de exclusão de uma entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="id">ID do registro</param>
        /// <returns>URL de exclusão (ex: "/Clientes/Delete/123")</returns>
        public static string GetDeleteUrl(string entityName, long id)
        {
            return GetActionUrl(entityName, "Delete", id);
        }

        /// <summary>
        /// Obtém a URL de exportação de uma entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="formato">Formato de exportação (opcional)</param>
        /// <returns>URL de exportação (ex: "/Clientes/Export?formato=csv")</returns>
        public static string GetExportUrl(string entityName, string? formato = null)
        {
            var url = GetActionUrl(entityName, "Export");

            if (!string.IsNullOrEmpty(formato))
            {
                url += $"?formato={formato}";
            }

            return url;
        }

        /// <summary>
        /// Obtém a URL AJAX para GetDataAjax de uma entidade
        /// </summary>
        /// <param name="entityName">Nome da entidade</param>
        /// <returns>URL AJAX (ex: "/Clientes/GetDataAjax")</returns>
        public static string GetAjaxUrl(string entityName)
        {
            return GetActionUrl(entityName, "GetDataAjax");
        }

        /// <summary>
        /// Obtém URLs para um tipo genérico
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <returns>Objeto com URLs úteis</returns>
        public static ControllerUrls GetUrls<T>()
        {
            var entityName = typeof(T).Name;
            return new ControllerUrls
            {
                EntityName = entityName,
                ControllerName = GetControllerName(entityName),
                Index = GetActionUrl(entityName, "Index"),
                Create = GetCreateUrl(entityName),
                Export = GetExportUrl(entityName),
                Ajax = GetAjaxUrl(entityName)
            };
        }

        /// <summary>
        /// Obtém URLs para uma entidade específica
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="id">ID da entidade</param>
        /// <returns>Objeto com URLs úteis</returns>
        public static EntityUrls GetEntityUrls<T>(long id)
        {
            var entityName = typeof(T).Name;
            return new EntityUrls
            {
                EntityName = entityName,
                ControllerName = GetControllerName(entityName),
                Id = id,
                Details = GetDetailsUrl(entityName, id),
                Edit = GetEditUrl(entityName, id),
                Delete = GetDeleteUrl(entityName, id)
            };
        }

        /// <summary>
        /// Obtém display name do controller
        /// </summary>
        public static string GetControllerDisplayName(Type entityType)
        {
            var controllerName = GetControllerName(entityType);
            return ConvertToDisplayName(controllerName);
        }

        private static string ConvertToDisplayName(string name)
        {
            // Converter CamelCase para palavras separadas
            var result = System.Text.RegularExpressions.Regex.Replace(
                name,
                "([a-z])([A-Z])",
                "$1 $2");

            return result;
        }
    }

    /// <summary>
    /// URLs úteis para um controller
    /// </summary>
    public class ControllerUrls
    {
        public string EntityName { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string Index { get; set; } = string.Empty;
        public string Create { get; set; } = string.Empty;
        public string Export { get; set; } = string.Empty;
        public string Ajax { get; set; } = string.Empty;
    }

    /// <summary>
    /// URLs úteis para uma entidade específica
    /// </summary>
    public class EntityUrls
    {
        public string EntityName { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public long Id { get; set; }
        public string Details { get; set; } = string.Empty;
        public string Edit { get; set; } = string.Empty;
        public string Delete { get; set; } = string.Empty;
    }
}