using MongoDB.Driver;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbRetailerRepository : BaseMongoDbRepository<Retailer>, IRetailerRepository
    {
        public MongoDbRetailerRepository(IMongoDbService mongoDbService) 
            : base(mongoDbService, "Retailer") { }

        public async Task<Retailer?> GetByNameAsync(string name)
        {
            var filter = Builders<Retailer>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression($"^{System.Text.RegularExpressions.Regex.Escape(name)}$", "i"));
            return await ExecuteSingleFilterAsync(filter);
        }

        public async Task<IEnumerable<Retailer>> GetByPriceGuaranteeDaysAsync(int minDays)
        {
            // Since PriceGuaranteeDays is stored as string, we need to convert for comparison
            var filter = Builders<Retailer>.Filter.Where(x => !string.IsNullOrEmpty(x.PriceGuaranteeDays) && int.Parse(x.PriceGuaranteeDays) >= minDays);
            return await ExecuteFilterAsync(filter);
        }
    }
}