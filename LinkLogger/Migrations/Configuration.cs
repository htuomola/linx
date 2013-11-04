using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using LinkLogger.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LinkLogger.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        private const string AdminRoleName = "Admin";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Roles.AddOrUpdate(r => r.Name, 
                new IdentityRole("LinkViewer"),
                new IdentityRole(AdminRoleName));
            
            /* Uncomment the following to have the database automatically seeded with admin account.
             * Do remember to change the password at first start!
             */
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //ApplicationUser adminUser = userManager.FindByName("Admin");
            //if (adminUser == null)
            //{
            //    var applicationUser = new ApplicationUser() { UserName = "Admin" };
            //    IdentityResult identityResultResult = userManager.Create(applicationUser);
            //    IdentityResult addPasswordResult = userManager.AddPassword(applicationUser.Id, "R4ndomP4ssHere23432!");
            //    IdentityResult addToRoleResult = userManager.AddToRole(applicationUser.Id, AdminRoleName);
            //}
        }
    }
}
