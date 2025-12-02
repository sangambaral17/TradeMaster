using TradeMaster.Core.Entities;

namespace TradeMaster.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TradeMasterDbContext context)
        {
            // This will create the database if it doesn't exist
            context.Database.EnsureCreated();

            // Check if we already have data
            if (context.Products.Any())
            {
                return; // DB has been seeded
            }

            // If not, the OnModelCreating seed data will be applied automatically by EnsureCreated
            // But we can add more complex logic here if needed
        }
    }
}
