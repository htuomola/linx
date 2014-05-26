using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LinkLogger.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
            : base("name=DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual DbSet<Link> Links { get; set; }
    }
}