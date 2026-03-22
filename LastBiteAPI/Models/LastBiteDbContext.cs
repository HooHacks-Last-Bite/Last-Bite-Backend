using Microsoft.EntityFrameworkCore;

namespace LastBiteAPI.Models
{
    public class LastBiteDbContext : DbContext
    {
        public LastBiteDbContext(DbContextOptions<LastBiteDbContext> options) : base(options) { }

        public DbSet<ScansModel> Scans { get; set; }

        public DbSet<MetricsModel> Metrics { get; set; }
    }
}
