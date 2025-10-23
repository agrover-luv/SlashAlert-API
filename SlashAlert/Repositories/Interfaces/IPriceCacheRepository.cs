using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IPriceCacheRepository : IRepository<PriceCache>
    {
        Task<PriceCache?> GetByUrlAsync(string url);
        Task<IEnumerable<PriceCache>> GetByProductNameAsync(string productName);
        Task<IEnumerable<PriceCache>> GetRecentlyCheckedAsync(int hours = 24);
        Task<IEnumerable<PriceCache>> GetDiscountedItemsAsync();
    }
}