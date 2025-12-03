using TradeMaster.Core.Entities;

namespace TradeMaster.Core.Interfaces
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetSalesWithDetailsAsync();
    }
}
