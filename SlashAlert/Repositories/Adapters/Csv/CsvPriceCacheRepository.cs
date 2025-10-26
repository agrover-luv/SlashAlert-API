using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvPriceCacheRepository : BaseCsvRepository<PriceCache>, IPriceCacheRepository
    {
        public CsvPriceCacheRepository(ICsvService csvService) : base(csvService, "PriceCache_export.csv") { }

        public async Task<PriceCache?> GetByUrlAsync(string url, string userEmail)
        {
            var priceCaches = await GetAllAsync(userEmail);
            return priceCaches.FirstOrDefault(pc => pc.Url == url);
        }

        public async Task<IEnumerable<PriceCache>> GetByProductNameAsync(string productName, string userEmail)
        {
            var priceCaches = await GetAllAsync(userEmail);
            return priceCaches.Where(pc => pc.ProductNameFound.Contains(productName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<PriceCache>> GetRecentlyCheckedAsync(string userEmail, int hours = 24)
        {
            var priceCaches = await GetAllAsync(userEmail);
            var cutoffTime = DateTime.UtcNow.AddHours(-hours);
            
            return priceCaches.Where(pc => 
            {
                if (DateTime.TryParse(pc.LastChecked, out var lastChecked))
                {
                    return lastChecked >= cutoffTime;
                }
                return false;
            });
        }

        public async Task<IEnumerable<PriceCache>> GetDiscountedItemsAsync(string userEmail)
        {
            var priceCaches = await GetAllAsync(userEmail);
            return priceCaches.Where(pc => 
            {
                if (decimal.TryParse(pc.DiscountAmount, out var discountAmount))
                {
                    return discountAmount > 0;
                }
                return false;
            });
        }
    }
}