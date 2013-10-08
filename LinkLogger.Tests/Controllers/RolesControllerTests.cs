using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using LinkLogger.Controllers;
using LinkLogger.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using IRole = Microsoft.AspNet.Identity.IRole;

namespace LinkLogger.Tests.Controllers
{
    [TestClass]
    public class RolesControllerTests
    {
        private AuthenticationIdentityManager _identityManager;
        private CancellationTokenSource _cts;

        public RolesControllerTests()
        {
            _identityManager = new AuthenticationIdentityManager(new IdentityStore());
            _cts = new CancellationTokenSource();
        }

        [TestInitialize]
        public void Init()
        {
            using (var ctx = new ApplicationDbContext())
            {
                ctx.Database.ExecuteSqlCommand("delete from dbo.AspNetUserManagement");
                ctx.UserRoles.RemoveRange(ctx.UserRoles.ToArray());
                ctx.Users.RemoveRange(ctx.Users.ToArray());

                var roles = ctx.Roles.ToArray();
                ctx.Roles.RemoveRange(roles);
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
            var targetRole = new Role("testing");
            await CreateRole(targetRole);

            await _identityManager.SaveChangesAsync(_cts.Token);

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
            var targetRole = new Role("testing");
            await CreateRole(targetRole);

            await _identityManager.SaveChangesAsync(_cts.Token);

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
            var role = new Role("testing");
            await CreateRole(role);

            var user = new User("tester");
            await CreateUser(user);
            var identityResult = await _identityManager.Roles.AddUserToRoleAsync(user.Id, role.Id);
            identityResult.Success.ShouldBeTrue();

            await _identityManager.SaveChangesAsync(_cts.Token);

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
            var targetRole = new Role("testing");
            await CreateRole(targetRole);

            var user = new User("tester");
            await CreateUser(user);

            await _identityManager.SaveChangesAsync(_cts.Token);

            var target = new RolesController();
            var actual = await target.EditMembers(targetRole.Id) as ViewResult;
            actual.ShouldNotBeNull();
            var model = actual.Model as EditRoleMembersViewModel;
            model.ShouldNotBeNull();
            model.AvailableUsers.Count().ShouldEqual(1);
            model.CurrentMembers.ShouldNotBeNull().ShouldBeEmpty();
        }

        private async Task CreateUser(User user)
        {
            var identityResult = await _identityManager.Users.CreateUserAsync(user, _cts.Token);
            identityResult.Success.ShouldBeTrue();
        }

        private async Task CreateRole(Role targetRole)
        {
            IdentityResult identityResult = await _identityManager.Roles.CreateRoleAsync(targetRole, _cts.Token);
            identityResult.Success.ShouldBeTrue();
        }
    }

    
}