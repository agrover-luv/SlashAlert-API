using SlashAlert.Api.Services;
using SlashAlert.Models;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Repositories.Adapters.Csv
{
    public class CsvReviewRepository : BaseCsvRepository<Review>, IReviewRepository
    {
        public CsvReviewRepository(ICsvService csvService) : base(csvService, "Review_export.csv") { }

        public async Task<IEnumerable<Review>> GetByRatingAsync(int rating, string userEmail)
        {
            var reviews = await GetAllAsync(userEmail);
            return reviews.Where(r => 
            {
                if (int.TryParse(r.Rating, out var reviewRating))
                {
                    return reviewRating == rating;
                }
                return false;
            });
        }

        public async Task<IEnumerable<Review>> GetVerifiedReviewsAsync(string userEmail)
        {
            var reviews = await GetAllAsync(userEmail);
            return reviews.Where(r => r.IsVerified == "true");
        }

        public async Task<IEnumerable<Review>> GetByUserNameAsync(string userName, string userEmail)
        {
            var reviews = await GetAllAsync(userEmail);
            return reviews.Where(r => r.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Review>> GetTopRatedReviewsAsync(string userEmail, int minRating = 4)
        {
            var reviews = await GetAllAsync(userEmail);
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