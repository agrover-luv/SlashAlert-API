using MongoDB.Driver;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;
using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbUserRepository : BaseMongoDbRepository<UserModel>, IUserRepository
    {
        public MongoDbUserRepository(IMongoDbService mongoDbService) 
            : base(mongoDbService, "User") { }

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var filter = Builders<UserModel>.Filter.Regex(x => x.Email, new MongoDB.Bson.BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(email)}$", "i"));
            return await ExecuteSingleFilterAsync(filter);
        }

        public async Task<UserModel?> GetBySubAsync(string sub)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Sub, sub);
            return await ExecuteSingleFilterAsync(filter);
        }

        public async Task<IEnumerable<UserModel>> GetByProviderAsync(string provider)
        {
            var filter = Builders<UserModel>.Filter.Regex(x => x.Provider, new MongoDB.Bson.BsonRegularExpression(provider, "i"));
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsersAsync()
        {
            var filter = Builders<UserModel>.Filter.Or(
                Builders<UserModel>.Filter.Eq(x => x.IsActive, true),
                Builders<UserModel>.Filter.Exists(x => x.IsActive, false)
            );
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var filter = Builders<UserModel>.Filter.Gte(x => x.LastLogin, cutoffDate);
            return await ExecuteFilterAsync(filter);
        }

        public async Task<UserModel?> GetByPartitionKeyAsync(string id, string partitionKey)
        {
            // For MongoDB, we'll use a combination of ID and a custom field for partition key
            var filter = Builders<UserModel>.Filter.And(
                Builders<UserModel>.Filter.Eq(x => x.Id, id),
                Builders<UserModel>.Filter.Eq(x => x.Provider, partitionKey) // Using provider as partition key
            );
            return await ExecuteSingleFilterAsync(filter);
        }
    }
}