using System.Web.Mvc;
using System.Web.Routing;

namespace LinkLogger.Controllers
{
    public class RoleFilterAttribute : AuthorizeAttribute
    {
        public RoleFilterAttribute(string roleName)
        {
            Roles = roleName;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    {"action", "RoleMissing"},
                    {"controller", "Home"}
                });
        }
    }
}