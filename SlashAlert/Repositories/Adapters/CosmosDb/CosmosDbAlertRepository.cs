using Microsoft.Azure.Cosmos;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.CosmosDb
{
    public class CosmosDbAlertRepository : BaseCosmosDbRepository<Alert>, IAlertRepository
    {
        public CosmosDbAlertRepository(Container container) : base(container) { }

        public async Task<IEnumerable<Alert>> GetByProductIdAsync(string productId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.product_id = @productId")
                .WithParameter("@productId", productId);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(string userId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.user_id = @userId")
                .WithParameter("@userId", userId);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetByAlertTypeAsync(string alertType)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.alert_type = @alertType")
                .WithParameter("@alertType", alertType);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetSentAlertsAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.email_sent = @emailSent OR c.sms_sent = @smsSent")
                .WithParameter("@emailSent", "true")
                .WithParameter("@smsSent", "true");
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetRecentAlertsAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.sent_at >= @cutoffDate")
                .WithParameter("@cutoffDate", cutoffDate.ToString("O"));
            return await ExecuteQueryAsync(queryDefinition);
        }
    }
}