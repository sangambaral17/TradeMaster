using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TradeMaster.Infrastructure.Data
{
    public class TradeMasterDbContextFactory : IDesignTimeDbContextFactory<TradeMasterDbContext>
    {
        public TradeMasterDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TradeMasterDbContext>();
            optionsBuilder.UseSqlite("Data Source=trademaster.db");

            return new TradeMasterDbContext(optionsBuilder.Options);
        }
    }
}
