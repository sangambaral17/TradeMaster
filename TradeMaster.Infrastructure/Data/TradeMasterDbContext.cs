using Microsoft.EntityFrameworkCore;
using TradeMaster.Core.Entities;

namespace TradeMaster.Infrastructure.Data
{
    public class TradeMasterDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        // Constructor for Dependency Injection
        public TradeMasterDbContext(DbContextOptions<TradeMasterDbContext> options) : base(options)
        {
        }

        // Configures the database (used when no DI container is available, e.g., migrations)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=trademaster.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some initial data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Gadgets and devices" },
                new Category { Id = 2, Name = "Groceries", Description = "Daily essentials" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 1200.00m, StockQuantity = 10, CategoryId = 1, Sku = "ELEC-001" },
                new Product { Id = 2, Name = "Smartphone", Price = 800.00m, StockQuantity = 20, CategoryId = 1, Sku = "ELEC-002" },
                new Product { Id = 3, Name = "Rice (5kg)", Price = 15.00m, StockQuantity = 50, CategoryId = 2, Sku = "GROC-001" }
            );
        }
    }
}
