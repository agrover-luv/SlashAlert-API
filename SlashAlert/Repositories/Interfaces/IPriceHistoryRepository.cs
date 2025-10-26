using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IPriceHistoryRepository : IRepository<PriceHistory>
    {
        Task<IEnumerable<PriceHistory>> GetByProductIdAsync(string productId, string createdBy);
        Task<IEnumerable<PriceHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string createdBy);
        Task<IEnumerable<PriceHistory>> GetPriceDropsAsync(string createdBy);
        Task<PriceHistory?> GetLatestPriceAsync(string productId, string createdBy);
    }
}