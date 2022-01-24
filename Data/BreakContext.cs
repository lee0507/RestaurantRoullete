using Microsoft.EntityFrameworkCore;
using Swilago.Data.Tables;

namespace Swilago.Data
{
    public class BreakContext : DbContext
    {
        public BreakContext(DbContextOptions<BreakContext> options) : base(options)
        {
            Database.SetCommandTimeout(3600);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TRestaurant>()
                .HasKey(k => new { k.RestaurantId });

            modelBuilder.Entity<TStatistics>()
                .HasKey(k => new { k.StatisticsId });
        }

        public DbSet<TRestaurant> Restaurant { get; set; }

        public DbSet<TStatistics> Statistics { get; set; }
    }
}
