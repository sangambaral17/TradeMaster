using Microsoft.EntityFrameworkCore;
using TradeMaster.Core.Entities;
using TradeMaster.Core.Interfaces;

namespace TradeMaster.Infrastructure.Data
{
    public class SaleRepository : EfRepository<Sale>, ISaleRepository
    {
        public SaleRepository(TradeMasterDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Sale>> GetSalesWithItemsAsync()
        {
            return await _context.Sales
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }
    }
}
