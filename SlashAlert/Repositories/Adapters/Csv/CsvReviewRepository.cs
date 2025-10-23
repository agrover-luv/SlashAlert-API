using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvReviewRepository : BaseCsvRepository<Review>, IReviewRepository
    {
        public CsvReviewRepository(ICsvService csvService) : base(csvService, "Review_export.csv") { }

        public async Task<IEnumerable<Review>> GetByRatingAsync(int rating)
        {
            var reviews = await GetAllAsync();
            return reviews.Where(r => 
            {
                if (int.TryParse(r.Rating, out var reviewRating))
                {
                    return reviewRating == rating;
                }
                return false;
            });
        }

        public async Task<IEnumerable<Review>> GetVerifiedReviewsAsync()
        {
            var reviews = await GetAllAsync();
            return reviews.Where(r => r.IsVerified == "true");
        }

        public async Task<IEnumerable<Review>> GetByUserNameAsync(string userName)
        {
            var reviews = await GetAllAsync();
            return reviews.Where(r => r.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Review>> GetTopRatedReviewsAsync(int minRating = 4)
        {
            var reviews = await GetAllAsync();
            return reviews.Where(r => 
            {
                if (int.TryParse(r.Rating, out var reviewRating))
                {
                    return reviewRating >= minRating;
                }
                return false;
            });
        }
    }
}