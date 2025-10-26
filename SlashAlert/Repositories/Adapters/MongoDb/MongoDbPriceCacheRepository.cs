using MongoDB.Driver;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbPriceCacheRepository : BaseMongoDbRepository<PriceCache>, IPriceCacheRepository
    {
        public MongoDbPriceCacheRepository(IMongoDbService mongoDbService) 
            : base(mongoDbService, "PriceCache") { }

        public async Task<PriceCache?> GetByUrlAsync(string url)
        {
            var filter = Builders<PriceCache>.Filter.Eq(x => x.Url, url);
            return await ExecuteSingleFilterAsync(filter);
        }

        public async Task<IEnumerable<PriceCache>> GetByProductNameAsync(string productName)
        {
            var filter = Builders<PriceCache>.Filter.Regex(x => x.ProductNameFound, new MongoDB.Bson.BsonRegularExpression(productName, "i"));
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<PriceCache>> GetRecentlyCheckedAsync(int hours = 24)
        {
            // Since LastChecked is stored as string, we'll need to handle this differently
            // For now, let's return all items and handle date filtering in application logic
            var filter = Builders<PriceCache>.Filter.Ne(x => x.LastChecked, null);
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<PriceCache>> GetDiscountedItemsAsync()
        {
            // Since we don't have DiscountPercentage field, let's check if there's a discount amount
            var filter = Builders<PriceCache>.Filter.And(
                Builders<PriceCache>.Filter.Ne(x => x.DiscountAmount, null),
                Builders<PriceCache>.Filter.Ne(x => x.DiscountAmount, ""),
                Builders<PriceCache>.Filter.Ne(x => x.DiscountAmount, "0")
            );
            return await ExecuteFilterAsync(filter);
        }
    }
}