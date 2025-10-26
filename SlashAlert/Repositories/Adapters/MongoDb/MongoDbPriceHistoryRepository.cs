using MongoDB.Driver;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbPriceHistoryRepository : BaseMongoDbRepository<PriceHistory>, IPriceHistoryRepository
    {
        public MongoDbPriceHistoryRepository(IMongoDbService mongoDbService) 
            : base(mongoDbService, "PriceHistory") { }

        public async Task<IEnumerable<PriceHistory>> GetByProductIdAsync(string productId)
        {
            var filter = Builders<PriceHistory>.Filter.Eq(x => x.ProductId, productId);
            var sort = Builders<PriceHistory>.Sort.Descending(x => x.Date);
            
            var result = await _collection.FindAsync(filter, new FindOptions<PriceHistory>
            {
                Sort = sort
            });
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<PriceHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Since Date is stored as string, we'll need to handle this in application logic
            // For now, return all items and let the application filter
            var filter = Builders<PriceHistory>.Filter.Ne(x => x.Date, null);
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<PriceHistory>> GetPriceDropsAsync()
        {
            // Since CurrentPrice and PreviousPrice don't exist in our model, 
            // we'll return items where ChangePercentage indicates a drop (negative value)
            var filter = Builders<PriceHistory>.Filter.Regex(x => x.ChangePercentage, new MongoDB.Bson.BsonRegularExpression("^-", ""));
            return await ExecuteFilterAsync(filter);
        }

        public async Task<PriceHistory?> GetLatestPriceAsync(string productId)
        {
            var filter = Builders<PriceHistory>.Filter.Eq(x => x.ProductId, productId);
            var sort = Builders<PriceHistory>.Sort.Descending(x => x.Date);
            
            var result = await _collection.FindAsync(filter, new FindOptions<PriceHistory>
            {
                Sort = sort,
                Limit = 1
            });
            return await result.FirstOrDefaultAsync();
        }
    }
}