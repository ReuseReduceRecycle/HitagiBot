using Microsoft.EntityFrameworkCore;

namespace HitagiBot.Data
{
    public class TelegramContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                $@"Server={Program.Config["MySQL:Server"]};Database={Program.Config["MySQL:Database Name"]};uid={
                        Program.Config["MySQL:Username"]
                    };pwd={Program.Config["MySQL:Password"]}");
        }
    }
}