using MongoDB.Driver;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbProductRepository : BaseMongoDbRepository<Product>, IProductRepository
    {
        public MongoDbProductRepository(IMongoDbService mongoDbService) 
            : base(mongoDbService, "Product") { }

        public async Task<IEnumerable<Product>> GetByRetailerAsync(string retailer)
        {
            var filter = Builders<Product>.Filter.Regex(x => x.Retailer, new MongoDB.Bson.BsonRegularExpression(retailer, "i"));
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            var filter = Builders<Product>.Filter.Regex(x => x.Category, new MongoDB.Bson.BsonRegularExpression(category, "i"));
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            var filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Eq(x => x.IsActive, "true"),
                Builders<Product>.Filter.Exists(x => x.IsActive, false)
            );
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Product>> GetByCreatedByIdAsync(string createdById)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.CreatedById, createdById);
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            // Since CurrentPrice is stored as string, we need to handle string-to-decimal conversion
            var filter = Builders<Product>.Filter.Where(x => 
                !string.IsNullOrEmpty(x.CurrentPrice) && 
                decimal.Parse(x.CurrentPrice) >= minPrice && 
                decimal.Parse(x.CurrentPrice) <= maxPrice);
            return await ExecuteFilterAsync(filter);
        }

        public async Task<Product?> GetByUrlAsync(string url)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Url, url);
            return await ExecuteSingleFilterAsync(filter);
        }
    }
}