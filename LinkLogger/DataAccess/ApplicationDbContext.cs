using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LinkLogger.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Link> Links { get; set; }
    }
}