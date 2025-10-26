using MongoDB.Driver;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbAlertRepository : BaseMongoDbRepository<Alert>, IAlertRepository
    {
        public MongoDbAlertRepository(IMongoDbService mongoDbService) 
            : base(mongoDbService, "Alert") { }

        public async Task<IEnumerable<Alert>> GetByProductIdAsync(string productId)
        {
            var filter = Builders<Alert>.Filter.Eq(x => x.ProductId, productId);
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<Alert>.Filter.Eq(x => x.UserId, userId);
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Alert>> GetByAlertTypeAsync(string alertType)
        {
            var filter = Builders<Alert>.Filter.Regex(x => x.AlertType, new MongoDB.Bson.BsonRegularExpression(alertType, "i"));
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Alert>> GetSentAlertsAsync()
        {
            var filter = Builders<Alert>.Filter.Eq(x => x.EmailSent, "true");
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Alert>> GetRecentAlertsAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var filter = Builders<Alert>.Filter.Gte(x => x.CreatedDate, cutoffDate);
            return await ExecuteFilterAsync(filter);
        }
    }
}