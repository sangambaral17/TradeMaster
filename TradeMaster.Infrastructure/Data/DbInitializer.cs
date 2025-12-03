using Microsoft.EntityFrameworkCore;
using TradeMaster.Core.Entities;

namespace TradeMaster.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(TradeMasterDbContext context)
        {
            // Apply pending migrations and create the database if it doesn't exist
            // This ensures the database schema matches the migration files
            context.Database.Migrate();

            // Check if we already have data
            if (context.Products.Any())
            {
                return; // DB has been seeded
            }

            // If not, the OnModelCreating seed data will be applied automatically by migrations
            // But we can add more complex logic here if needed
        }
    }
}
