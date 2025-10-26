using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IPriceCacheRepository : IRepository<PriceCache>
    {
        Task<PriceCache?> GetByUrlAsync(string url, string createdBy);
        Task<IEnumerable<PriceCache>> GetByProductNameAsync(string productName, string createdBy);
        Task<IEnumerable<PriceCache>> GetRecentlyCheckedAsync(string createdBy, int hours = 24);
        Task<IEnumerable<PriceCache>> GetDiscountedItemsAsync(string createdBy);
    }
}