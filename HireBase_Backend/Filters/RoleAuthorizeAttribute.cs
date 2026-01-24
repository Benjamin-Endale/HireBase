using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace HireBase_Backend.Filters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(string roles)
        {
            // Split roles by comma and trim any extra spaces
            _roles = roles.Split(',')
                          .Select(r => r.Trim())
                          .ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if the user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new JsonResult(new
                {
                    message = "Access denied. User is not authenticated."
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // Check if user has at least one of the allowed roles
            bool hasRole = _roles.Any(role => user.IsInRole(role));

            if (!hasRole)
            {
                context.Result = new JsonResult(new
                {
                    message = $"Access denied. Only {string.Join(", ", _roles)} can perform this action."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
