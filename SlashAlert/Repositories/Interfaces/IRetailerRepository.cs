using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IRetailerRepository : IRepository<Retailer>
    {
        Task<Retailer?> GetByNameAsync(string name, string createdBy);
        Task<IEnumerable<Retailer>> GetByPriceGuaranteeDaysAsync(int minDays, string createdBy);
    }
}