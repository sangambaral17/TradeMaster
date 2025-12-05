using Microsoft.EntityFrameworkCore;
using TradeMaster.Core.Entities;
using TradeMaster.Infrastructure.Data;

namespace TradeMaster.Tests.Helpers;

/// <summary>
/// Factory for creating in-memory test databases
/// </summary>
public static class TestDatabaseFactory
{
    /// <summary>
    /// Creates a new in-memory database context for testing
    /// </summary>
    public static TradeMasterDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TradeMasterDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TradeMasterDbContext(options);
        context.Database.EnsureCreated();
        
        return context;
    }

    /// <summary>
    /// Creates a context with seed data for testing
    /// </summary>
    public static TradeMasterDbContext CreateContextWithSeedData()
    {
        var context = CreateInMemoryContext();
        SeedTestData(context);
        return context;
    }

    /// <summary>
    /// Seeds the database with test data
    /// </summary>
    private static void SeedTestData(TradeMasterDbContext context)
    {
        // Add test categories
        var categories = new[]
        {
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Clothing" },
            new Category { Id = 3, Name = "Food" }
        };
        context.Categories.AddRange(categories);

        // Add test products
        var products = new[]
        {
            new Product 
            { 
                Id = 1, 
                Name = "Test Product 1", 
                Sku = "TEST001",
                CategoryId = 1, 
                Price = 100.00m, 
                Currency = "NPR",
                StockQuantity = 50 
            },
            new Product 
            { 
                Id = 2, 
                Name = "Test Product 2", 
                Sku = "TEST002",
                CategoryId = 2, 
                Price = 200.00m, 
                Currency = "NPR",
                StockQuantity = 30 
            },
            new Product 
            { 
                Id = 3, 
                Name = "Test Product 3", 
                Sku = "TEST003",
                CategoryId = 3, 
                Price = 50.00m, 
                Currency = "NPR",
                StockQuantity = 100 
            }
        };
        context.Products.AddRange(products);

        context.SaveChanges();
    }

    /// <summary>
    /// Cleans up the context
    /// </summary>
    public static void Cleanup(TradeMasterDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
