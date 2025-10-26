using MongoDB.Driver;
using MongoDB.Bson;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;
using UserModel = SlashAlert.Models.User;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbUserRepository : IUserRepository
    {
        protected readonly IMongoCollection<UserModel> _collection;
        protected readonly IMongoDbService _mongoDbService;

        public MongoDbUserRepository(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            _collection = mongoDbService.GetCollection<UserModel>("User");
        }

        // IRepository<User> implementation - Users don't need createdBy filtering
        public virtual async Task<IEnumerable<UserModel>> GetAllAsync(string createdBy)
        {
            // For users, we return all users (this might be restricted in real scenarios)
            var filter = Builders<UserModel>.Filter.Empty;
            var result = await _collection.FindAsync(filter);
            return await result.ToListAsync();
        }

        public virtual async Task<UserModel?> GetByIdAsync(string id, string createdBy)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, id);
            var result = await _collection.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        public virtual async Task<UserModel> CreateAsync(UserModel entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = ObjectId.GenerateNewId().ToString();
            }
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;

            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<UserModel> UpdateAsync(UserModel entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            
            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, entity.Id);
            var result = await _collection.ReplaceOneAsync(filter, entity);
            
            if (result.MatchedCount == 0)
            {
                throw new InvalidOperationException($"User with ID {entity.Id} not found for update");
            }
            
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(string id, string createdBy)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public virtual async Task<bool> ExistsAsync(string id, string createdBy)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Id, id);
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public virtual async Task<int> CountAsync(string createdBy)
        {
            var count = await _collection.CountDocumentsAsync(Builders<UserModel>.Filter.Empty);
            return (int)count;
        }

        // IUserRepository specific methods
        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var filter = Builders<UserModel>.Filter.Regex(x => x.Email, new MongoDB.Bson.BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(email)}$", "i"));
            var result = await _collection.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<UserModel?> GetBySubAsync(string sub)
        {
            var filter = Builders<UserModel>.Filter.Eq(x => x.Sub, sub);
            var result = await _collection.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserModel>> GetByProviderAsync(string provider)
        {
            var filter = Builders<UserModel>.Filter.Regex(x => x.Provider, new MongoDB.Bson.BsonRegularExpression(provider, "i"));
            var result = await _collection.FindAsync(filter);
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> GetActiveUsersAsync()
        {
            var filter = Builders<UserModel>.Filter.Or(
                Builders<UserModel>.Filter.Eq(x => x.IsActive, true),
                Builders<UserModel>.Filter.Exists(x => x.IsActive, false)
            );
            var result = await _collection.FindAsync(filter);
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> GetRecentLoginsAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var filter = Builders<UserModel>.Filter.Gte(x => x.LastLogin, cutoffDate);
            var result = await _collection.FindAsync(filter);
            return await result.ToListAsync();
        }

        public async Task<UserModel?> GetByPartitionKeyAsync(string id, string partitionKey)
        {
            // For MongoDB, we'll use a combination of ID and a custom field for partition key
            var filter = Builders<UserModel>.Filter.And(
                Builders<UserModel>.Filter.Eq(x => x.Id, id),
                Builders<UserModel>.Filter.Eq(x => x.Provider, partitionKey) // Using provider as partition key
            );
            var result = await _collection.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }
    }
}