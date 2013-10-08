using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using LinkLogger.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace LinkLogger.Controllers
{
    public class RolesController : Controller
    {
        public async Task<ActionResult> Index()
        {
            Role[] roles;
            using (var ctx = new ApplicationDbContext())
            {
                roles = await ctx.Roles.ToArrayAsync();
            }

            var roleVms = roles.Select(MapDbRoleToRoleVm);
            return View(roleVms);
        }

        public async Task<ActionResult> EditMembers(string id)
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var role = await ctx.Roles.FindAsync(id);
                    var currentMembers = role.UserRoles.Select(ur => ur.User).ToArray();
                    var currentMemberIds = currentMembers.Select(u => u.Id);

                    var availableUsers = await ctx.Users.Where(u => !currentMemberIds.Contains(u.Id)).ToArrayAsync();
                
                    var model = new EditRoleMembersViewModel()
                                {
                                    AvailableUsers = availableUsers.Select(MapDbUserToUserViewModel),
                                    CurrentMembers = currentMembers.Select(MapDbUserToUserViewModel),
                                    RoleName = role.Name,
                                    RoleId =id
                                };

                    return View(model);
                }
            }
            catch (Exception e)
            {
                // TODO
                return View(new EditRoleMembersViewModel());
            }
        }

        private static RoleViewModel MapDbRoleToRoleVm(Role arg)
        {
            return new RoleViewModel() {Name = arg.Name, Id = arg.Id};
        }

        private static UserViewModel MapDbUserToUserViewModel(User u)
        {
            return new UserViewModel {Id = u.Id, UserName = u.UserName};
        }

        [HttpPost]
        public async Task<ActionResult> AddUserToRole(AddUserToRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityManager = new AuthenticationIdentityManager(new IdentityStore());
                var cts = new CancellationTokenSource();
                var identityResult = await identityManager.Roles.AddUserToRoleAsync(model.UserId, model.RoleId, cts.Token);
                
                if (identityResult.Success)
                {
                    await identityManager.SaveChangesAsync(cts.Token);
                    TempData["Message"] = "User added to role succesfully";
                }
                else
                {
                    // TODO: add errors to TempData
                }
            }

            return RedirectToAction("EditMembers", "Roles", model.RoleId);
        }
    }
}