using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IAlertRepository : IRepository<Alert>
    {
        Task<IEnumerable<Alert>> GetByProductIdAsync(string productId);
        Task<IEnumerable<Alert>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Alert>> GetByAlertTypeAsync(string alertType);
        Task<IEnumerable<Alert>> GetSentAlertsAsync();
        Task<IEnumerable<Alert>> GetRecentAlertsAsync(int days = 30);
    }
}