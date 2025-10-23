using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByRatingAsync(int rating);
        Task<IEnumerable<Review>> GetVerifiedReviewsAsync();
        Task<IEnumerable<Review>> GetByUserNameAsync(string userName);
        Task<IEnumerable<Review>> GetTopRatedReviewsAsync(int minRating = 4);
    }
}