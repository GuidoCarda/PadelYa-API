using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using padelya_api.Services;

namespace padelya_api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireModuleAccessAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string _moduleName;

        public RequireModuleAccessAttribute(string moduleName)
        {
            _moduleName = moduleName;
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

            if (!await permissionService.HasModuleAccessAsync(userId, _moduleName))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}