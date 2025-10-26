using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvAlertRepository : BaseCsvRepository<Alert>, IAlertRepository
    {
        public CsvAlertRepository(ICsvService csvService) : base(csvService, "Alert_export.csv") { }

        public async Task<IEnumerable<Alert>> GetByProductIdAsync(string productId, string userEmail)
        {
            var alerts = await GetAllAsync(userEmail);
            return alerts.Where(a => a.ProductId == productId);
        }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(string userId, string userEmail)
        {
            var alerts = await GetAllAsync(userEmail);
            return alerts.Where(a => a.UserId == userId);
        }

        public async Task<IEnumerable<Alert>> GetByAlertTypeAsync(string alertType, string userEmail)
        {
            var alerts = await GetAllAsync(userEmail);
            return alerts.Where(a => a.AlertType == alertType);
        }

        public async Task<IEnumerable<Alert>> GetSentAlertsAsync(string userEmail)
        {
            var alerts = await GetAllAsync(userEmail);
            return alerts.Where(a => a.EmailSent == "true" || a.SmsSent == "true");
        }

        public async Task<IEnumerable<Alert>> GetRecentAlertsAsync(string userEmail, int days = 30)
        {
            var alerts = await GetAllAsync(userEmail);
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            return alerts.Where(a => 
            {
                if (DateTime.TryParse(a.SentAt, out var sentAt))
                {
                    return sentAt >= cutoffDate;
                }
                return false;
            });
        }
    }
}