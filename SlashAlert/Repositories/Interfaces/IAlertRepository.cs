using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IAlertRepository : IRepository<Alert>
    {
        Task<IEnumerable<Alert>> GetByProductIdAsync(string productId, string createdBy);
        Task<IEnumerable<Alert>> GetByUserIdAsync(string userId, string createdBy);
        Task<IEnumerable<Alert>> GetByAlertTypeAsync(string alertType, string createdBy);
        Task<IEnumerable<Alert>> GetSentAlertsAsync(string createdBy);
        Task<IEnumerable<Alert>> GetRecentAlertsAsync(string createdBy, int days = 30);
    }
}