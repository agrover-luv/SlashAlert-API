using Microsoft.Azure.Cosmos;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.CosmosDb
{
    public class CosmosDbAlertRepository : BaseCosmosDbRepository<Alert>, IAlertRepository
    {
        public CosmosDbAlertRepository(Container container) : base(container) { }

        public async Task<IEnumerable<Alert>> GetByProductIdAsync(string productId, string userEmail)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.product_id = @productId AND c.user_email = @userEmail")
                .WithParameter("@productId", productId)
                .WithParameter("@userEmail", userEmail);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(string userId, string userEmail)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.user_id = @userId AND c.user_email = @userEmail")
                .WithParameter("@userId", userId)
                .WithParameter("@userEmail", userEmail);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetByAlertTypeAsync(string alertType, string userEmail)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.alert_type = @alertType AND c.user_email = @userEmail")
                .WithParameter("@alertType", alertType)
                .WithParameter("@userEmail", userEmail);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetSentAlertsAsync(string userEmail)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE (c.email_sent = @emailSent OR c.sms_sent = @smsSent) AND c.user_email = @userEmail")
                .WithParameter("@emailSent", "true")
                .WithParameter("@smsSent", "true")
                .WithParameter("@userEmail", userEmail);
            return await ExecuteQueryAsync(queryDefinition);
        }

        public async Task<IEnumerable<Alert>> GetRecentAlertsAsync(string userEmail, int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.sent_at >= @cutoffDate AND c.user_email = @userEmail")
                .WithParameter("@cutoffDate", cutoffDate.ToString("O"))
                .WithParameter("@userEmail", userEmail);
            return await ExecuteQueryAsync(queryDefinition);
        }
    }
}