using SlashAlert.Models;

namespace SlashAlert.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByRatingAsync(int rating, string createdBy);
        Task<IEnumerable<Review>> GetVerifiedReviewsAsync(string createdBy);
        Task<IEnumerable<Review>> GetByUserNameAsync(string userName, string createdBy);
        Task<IEnumerable<Review>> GetTopRatedReviewsAsync(string createdBy, int minRating = 4);
    }
}