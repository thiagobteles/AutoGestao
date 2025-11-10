using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FGT.Atributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class RequiredRolesAttribute(params string[] roles) : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles = roles;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (_roles.Length == 0)
            {
                return;
            }

            var hasRole = _roles.Any(role => context.HttpContext.User.IsInRole(role));

            if (!hasRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}