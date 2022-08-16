using Microsoft.EntityFrameworkCore;

namespace InventoryService.Model

{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Pallet> Pallets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().ToTable("Items", "Inventory");
            modelBuilder.Entity<Pallet>().ToTable("Pallets", "Inventory");
        }
    }
}