using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IRetailerRepository : IRepository<Retailer>
    {
        Task<Retailer?> GetByNameAsync(string name);
        Task<IEnumerable<Retailer>> GetByPriceGuaranteeDaysAsync(int minDays);
    }
}