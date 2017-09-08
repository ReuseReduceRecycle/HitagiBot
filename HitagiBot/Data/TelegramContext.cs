using Microsoft.EntityFrameworkCore;

namespace HitagiBot.Data
{
    public class TelegramContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=hitagi.db");
        }
    }
}