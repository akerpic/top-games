using Microsoft.EntityFrameworkCore;
using TopGames.Models;

namespace TopGames.EntityFramework
{
    public class HerokuDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
            modelBuilder.Entity<Game>()
            .Property(b => b.created_date)
            .HasColumnType("timestamp without time zone")
            .ValueGeneratedOnAdd();
        }
        public HerokuDbContext(DbContextOptions<HerokuDbContext> options) :
            base(options)
        {
        }
        
        public DbSet<Game> game { get; set; }
    }
}