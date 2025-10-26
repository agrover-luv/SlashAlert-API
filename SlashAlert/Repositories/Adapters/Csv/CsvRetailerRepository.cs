using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvRetailerRepository : BaseCsvRepository<Retailer>, IRetailerRepository
    {
        public CsvRetailerRepository(ICsvService csvService) : base(csvService, "Retailer_export.csv") { }

        public async Task<Retailer?> GetByNameAsync(string name, string userEmail)
        {
            var retailers = await GetAllAsync(userEmail);
            return retailers.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Retailer>> GetByPriceGuaranteeDaysAsync(int minDays, string userEmail)
        {
            var retailers = await GetAllAsync(userEmail);
            return retailers.Where(r => 
            {
                if (int.TryParse(r.PriceGuaranteeDays, out var days))
                {
                    return days >= minDays;
                }
                return false;
            });
        }
    }
}