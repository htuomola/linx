using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LinkLogger.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LinkLogger.Controllers
{
    /// <summary>
    /// RoleFilterAttribute is an authorization attribute which is used 
    /// to enforce role-based authorization using the new ASP.net MVC 5 
    /// Simple membership classes.
    /// </summary>
    public class RoleFilterAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Create a new instance of RoleFilterAttribute
        /// </summary>
        /// <param name="roleName">Name of the role to restrict access to.</param>
        public RoleFilterAttribute(string roleName)
        {
            Roles = roleName;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));
                if (!httpContext.User.Identity.IsAuthenticated) return false;

                var userId = httpContext.User.Identity.GetUserId();
                var isInRole = userManager.IsInRole(userId, Roles);
                return isInRole;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    {"action", "AccessDenied"},
                    {"controller", "Home"},
                    {"id", Roles}
                });
        }
    }
}