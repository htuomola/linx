using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LinkLogger.Controllers;
using LinkLogger.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace LinkLogger.Tests.Controllers
{
    [TestClass]
    public class RolesControllerTests
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesControllerTests()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        }

        [TestInitialize]
        public void Init()
        {
            using (var ctx = new ApplicationDbContext())
            {
                //ctx.Database.ExecuteSqlCommand("delete from dbo.AspNetUserManagement");
                foreach (var user in ctx.Users.ToArray())
                {
                    ctx.Users.Remove(user);
                }

                foreach (var role in ctx.Roles.ToArray())
                {
                    ctx.Roles.Remove(role);
                }

                ctx.SaveChanges();
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
        }

        [TestMethod]
        public async Task ListRoles_NoRoles_Ok()
        {
            var target = new RolesController();
            var actual = (ViewResult)(await target.Index());
            var roles = actual.Model as IEnumerable<RoleViewModel>;
            roles.ShouldNotBeNull().ShouldBeEmpty();
        }

        [TestMethod]
        public async Task ListRoles_1Role_Ok()
        {
            var targetRole = new IdentityRole("testing");
            await _roleManager.CreateAsync(targetRole);
            
            var target = new RolesController();
            var actual = (ViewResult)(await target.Index());
            var roles = actual.Model as IEnumerable<RoleViewModel>;
            roles.ShouldNotBeNull();
            roles.Count().ShouldEqual(1);

            var firstRole = roles.First();
            firstRole.Name.ShouldEqual(targetRole.Name);
            firstRole.Id.ShouldEqual(targetRole.Id);
        }

        [TestMethod]
        public async Task EditMembers_NoMembers_Ok()
        {
            var targetRole = new IdentityRole("testing");
            await _roleManager.CreateAsync(targetRole);
            
            var target = new RolesController();
            var actual = await target.EditMembers(targetRole.Id) as ViewResult;
            actual.ShouldNotBeNull();
            var model = actual.Model as EditRoleMembersViewModel;
            model.ShouldNotBeNull();
            model.AvailableUsers.ShouldNotBeNull().ShouldBeEmpty();
            model.CurrentMembers.ShouldNotBeNull().ShouldBeEmpty();
        }

        [TestMethod]
        public async Task EditMembers_1Member_Ok()
        {
            var role = new IdentityRole("testing");
            await _roleManager.CreateAsync(role);

            var user = new ApplicationUser() { UserName = "tester" };
            await _userManager.CreateAsync(user);
            
            var identityResult = await _userManager.AddToRoleAsync(user.Id, role.Id);
            identityResult.Succeeded.ShouldBeTrue();

            var target = new RolesController();
            var actual = await target.EditMembers(role.Id) as ViewResult;
            actual.ShouldNotBeNull();
            var model = actual.Model as EditRoleMembersViewModel;
            model.ShouldNotBeNull();
            model.AvailableUsers.ShouldNotBeNull().ShouldBeEmpty();
            model.CurrentMembers.Count().ShouldEqual(1);
        }

        [TestMethod]
        public async Task EditMembers_1User_NotMember_Ok()
        {
            var targetRole = new IdentityRole("testing");
            await _roleManager.CreateAsync(targetRole);

            var user = new ApplicationUser(){ UserName = "tester"};
            await _userManager.CreateAsync(user);

            var target = new RolesController();
            var actual = await target.EditMembers(targetRole.Id) as ViewResult;
            actual.ShouldNotBeNull();
            var model = actual.Model as EditRoleMembersViewModel;
            model.ShouldNotBeNull();
            model.AvailableUsers.Count().ShouldEqual(1);
            model.CurrentMembers.ShouldNotBeNull().ShouldBeEmpty();
        }
    }

    
}