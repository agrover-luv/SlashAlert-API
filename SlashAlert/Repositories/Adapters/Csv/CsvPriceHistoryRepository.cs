using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvPriceHistoryRepository : BaseCsvRepository<PriceHistory>, IPriceHistoryRepository
    {
        public CsvPriceHistoryRepository(ICsvService csvService) : base(csvService, "PriceHistory_export.csv") { }

        public async Task<IEnumerable<PriceHistory>> GetByProductIdAsync(string productId)
        {
            var priceHistories = await GetAllAsync();
            return priceHistories.Where(ph => ph.ProductId == productId);
        }

        public async Task<IEnumerable<PriceHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var priceHistories = await GetAllAsync();
            return priceHistories.Where(ph => 
            {
                if (DateTime.TryParse(ph.Date, out var date))
                {
                    return date >= startDate && date <= endDate;
                }
                return false;
            });
        }

        public async Task<IEnumerable<PriceHistory>> GetPriceDropsAsync()
        {
            var priceHistories = await GetAllAsync();
            return priceHistories.Where(ph => 
            {
                if (decimal.TryParse(ph.ChangePercentage, out var changePercentage))
                {
                    return changePercentage < 0;
                }
                return false;
            });
        }

        public async Task<PriceHistory?> GetLatestPriceAsync(string productId)
        {
            var priceHistories = await GetByProductIdAsync(productId);
            return priceHistories
                .Where(ph => DateTime.TryParse(ph.Date, out _))
                .OrderByDescending(ph => DateTime.Parse(ph.Date))
                .FirstOrDefault();
        }
    }
}