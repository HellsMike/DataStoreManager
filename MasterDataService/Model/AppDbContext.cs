using Microsoft.EntityFrameworkCore;

namespace MasterDataService.Model
    
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<KafkaItemMessage> Events { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().ToTable("Items", "MDS");
            modelBuilder.Entity<KafkaItemMessage>().ToTable("Events", "MDS");
        }
    }
}
