using Microsoft.AspNetCore.Mvc;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for managing review data from multiple sources (Cosmos DB containers, SQL tables, or CSV files)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return review != null ? Ok(review) : NotFound();
        }

        [HttpGet("rating/{rating}")]
        public async Task<IActionResult> GetByRating(int rating)
        {
            var reviews = await _reviewRepository.GetByRatingAsync(rating);
            return Ok(reviews);
        }

        [HttpGet("verified")]
        public async Task<IActionResult> GetVerifiedReviews()
        {
            var reviews = await _reviewRepository.GetVerifiedReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            var reviews = await _reviewRepository.GetByUserNameAsync(userName);
            return Ok(reviews);
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedReviews([FromQuery] int minRating = 4)
        {
            var reviews = await _reviewRepository.GetTopRatedReviewsAsync(minRating);
            return Ok(reviews);
        }
    }
}
