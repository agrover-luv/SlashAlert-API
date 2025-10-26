using MongoDB.Driver;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;
using SlashAlert.Services;

namespace SlashAlert.Repositories.Adapters.MongoDb
{
    public class MongoDbReviewRepository : BaseMongoDbRepository<Review>, IReviewRepository
    {
        public MongoDbReviewRepository(IMongoDbService mongoDbService) 
            : base(mongoDbService, "Review") { }

        public async Task<IEnumerable<Review>> GetByRatingAsync(int rating)
        {
            var filter = Builders<Review>.Filter.Eq(x => x.Rating, rating.ToString());
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Review>> GetVerifiedReviewsAsync()
        {
            var filter = Builders<Review>.Filter.Eq(x => x.IsVerified, "true");
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Review>> GetByUserNameAsync(string userName)
        {
            var filter = Builders<Review>.Filter.Regex(x => x.UserName, new MongoDB.Bson.BsonRegularExpression(userName, "i"));
            return await ExecuteFilterAsync(filter);
        }

        public async Task<IEnumerable<Review>> GetTopRatedReviewsAsync(int minRating = 4)
        {
            // Since Rating is stored as string, we need to handle string comparison
            var filter = Builders<Review>.Filter.Where(x => !string.IsNullOrEmpty(x.Rating) && int.Parse(x.Rating) >= minRating);
            return await ExecuteFilterAsync(filter);
        }
    }
}