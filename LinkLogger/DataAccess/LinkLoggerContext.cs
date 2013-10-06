using System.Data.Entity;

namespace LinkLogger.DataAccess
{
    public class LinkLoggerContext : DbContext
    {
        public LinkLoggerContext()
            : base("name=DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        public virtual DbSet<Link> Links { get; set; }
    }
}