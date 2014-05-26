using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LinkLogger.DataAccess;
using LinkLogger.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LinkLogger.Controllers
{
    [Authorize]
    [RoleFilter("Admin")]
    public class RolesController : Controller
    {
        public async Task<ActionResult> Index()
        {
            IdentityRole[] roles;
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
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
                    IdentityRole role = await roleManager.FindByNameAsync(id);
                    if (role == null)
                    {
                        // TODO: show error.
                    }

                    var roleMemberIds = role.Users.Select(m => m.UserId);

                    var roleMembers = from u in ctx.Users
                        where roleMemberIds.Contains(u.Id)
                        select u;

                    var availableUsers = await ctx.Users.Where(u => !roleMemberIds.Contains(u.Id)).ToArrayAsync();

                    var model = new EditRoleMembersViewModel()
                                {
                                    AvailableUsers = availableUsers.Select(MapDbUserToUserViewModel).ToArray(),
                                    CurrentMembers = roleMembers.Select(MapDbUserToUserViewModel).ToArray(),
                                    RoleName = role.Name,
                                    RoleId = role.Id
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

        private static RoleViewModel MapDbRoleToRoleVm(IdentityRole arg)
        {
            return new RoleViewModel() {Name = arg.Name, Id = arg.Id};
        }

        private static UserViewModel MapDbUserToUserViewModel(IdentityUser u)
        {
            return new UserViewModel {Id = u.Id, UserName = u.UserName};
        }

        [HttpPost]
        public async Task<ActionResult> AddUserToRole(AddUserToRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));
                    var identityResult = await userManager.AddToRoleAsync(model.UserId, model.RoleName);

                    if (identityResult.Succeeded)
                    {
                        TempData["Message"] = "User added to role succesfully";
                    }
                    else
                    {
                        // TODO: add errors to TempData
                    }
                }
            }

            return RedirectToAction("EditMembers", "Roles", new { Id = model.RoleName});
        }
    }
}