using AutoGestao.Atributes;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.Reflection;
using System.Security.Claims;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Helper centralizado para gerenciar campos de EmpresaCliente
    /// Aplica regras automáticas de visibilidade e preenchimento quando o usuário está logado em uma única empresa
    /// </summary>
    public static class EmpresaClienteFieldHelper
    {
        /// <summary>
        /// Verifica se um campo deve ser ocultado automaticamente por ser campo de EmpresaCliente
        /// quando o usuário não-admin está logado em apenas uma empresa
        /// </summary>
        public static bool ShouldHideEmpresaClienteField(PropertyInfo property, ClaimsPrincipal user, out long? empresaClienteId)
        {
            empresaClienteId = null;

            // Se o usuário é Admin, sempre mostrar o campo
            if (user.IsInRole("Admin"))
            {
                return false;
            }

            // Verificar se o usuário tem EmpresaClienteId (está logado em uma empresa)
            var empresaClienteIdClaim = user.FindFirst("EmpresaClienteId")?.Value;
            if (!long.TryParse(empresaClienteIdClaim, out var empresaId))
            {
                return false; // Usuário sem empresa logada, não esconder
            }

            empresaClienteId = empresaId;

            // Verificar se o campo é um campo de referência a EmpresaCliente
            return IsEmpresaClienteReferenceField(property);
        }

        /// <summary>
        /// Verifica se a propriedade é uma referência para EmpresaCliente
        /// </summary>
        public static bool IsEmpresaClienteReferenceField(PropertyInfo property)
        {
            // Verificar pelo atributo FormField
            var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();
            if (formFieldAttr != null &&
                formFieldAttr.Type == EnumFieldType.Reference &&
                formFieldAttr.Reference == typeof(EmpresaCliente))
            {
                return true;
            }

            // Verificar pelo nome da propriedade
            if (property.Name == "EmpresaClienteId" || property.Name == "IdEmpresaCliente")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Força o valor de EmpresaClienteId em uma entidade quando o usuário não-admin está logado
        /// </summary>
        public static void ForceEmpresaClienteId<T>(T entity, ClaimsPrincipal user) where T : class
        {
            if (user.IsInRole("Admin"))
            {
                return; // Admin pode escolher qualquer empresa
            }

            var empresaClienteIdClaim = user.FindFirst("EmpresaClienteId")?.Value;
            if (!long.TryParse(empresaClienteIdClaim, out var empresaClienteId))
            {
                return; // Usuário sem empresa logada
            }

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (IsEmpresaClienteReferenceField(property) && property.CanWrite)
                {
                    // Setar o valor forçado
                    if (property.PropertyType == typeof(long))
                    {
                        property.SetValue(entity, empresaClienteId);
                    }
                    else if (property.PropertyType == typeof(long?))
                    {
                        property.SetValue(entity, (long?)empresaClienteId);
                    }
                }
            }
        }

        /// <summary>
        /// Obtém o ID da empresa cliente do usuário logado (se houver)
        /// </summary>
        public static long? GetCurrentEmpresaClienteId(ClaimsPrincipal user)
        {
            var empresaClienteIdClaim = user.FindFirst("EmpresaClienteId")?.Value;
            return long.TryParse(empresaClienteIdClaim, out var empresaClienteId)
                ? empresaClienteId
                : null;
        }

        /// <summary>
        /// Verifica se o usuário deve ter acesso restrito (não-admin com empresa única)
        /// </summary>
        public static bool HasRestrictedAccess(ClaimsPrincipal user)
        {
            return !user.IsInRole("Admin") && GetCurrentEmpresaClienteId(user).HasValue;
        }
    }
}
