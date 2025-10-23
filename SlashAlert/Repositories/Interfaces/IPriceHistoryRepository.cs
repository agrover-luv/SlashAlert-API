using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IPriceHistoryRepository : IRepository<PriceHistory>
    {
        Task<IEnumerable<PriceHistory>> GetByProductIdAsync(string productId);
        Task<IEnumerable<PriceHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PriceHistory>> GetPriceDropsAsync();
        Task<PriceHistory?> GetLatestPriceAsync(string productId);
    }
}