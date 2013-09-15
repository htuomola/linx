using System.Data.Entity;

namespace LinkLogger.DataAccess
{
    public class LinkLoggerContext : DbContext
    {
        public LinkLoggerContext()
            : base("name=LinkLoggerContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        public virtual DbSet<Link> Links { get; set; }
    }
}