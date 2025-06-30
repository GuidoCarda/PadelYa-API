using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using padelya_api.Services;

namespace padelya_api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireAnyPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _permissions;

        public RequireAnyPermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionService = context.HttpContext.RequestServices
                .GetRequiredService<IPermissionService>();

            var userId = int.Parse(user.FindFirst("user_id")?.Value ?? "0");

            // Usar el método optimizado del servicio
            if (!await permissionService.HasAnyPermissionAsync(userId, _permissions))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Acceso permitido - tiene al menos un permiso
        }
    }
}