using Microsoft.AspNetCore.Mvc;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for managing price history data from multiple sources (Cosmos DB containers, SQL tables, or CSV files)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PriceHistoryController : ControllerBase
    {
        private readonly IPriceHistoryRepository _priceHistoryRepository;

        public PriceHistoryController(IPriceHistoryRepository priceHistoryRepository)
        {
            _priceHistoryRepository = priceHistoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var priceHistories = await _priceHistoryRepository.GetAllAsync();
            return Ok(priceHistories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var priceHistory = await _priceHistoryRepository.GetByIdAsync(id);
            return priceHistory != null ? Ok(priceHistory) : NotFound();
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(string productId)
        {
            var priceHistories = await _priceHistoryRepository.GetByProductIdAsync(productId);
            return Ok(priceHistories);
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var priceHistories = await _priceHistoryRepository.GetByDateRangeAsync(startDate, endDate);
            return Ok(priceHistories);
        }

        [HttpGet("price-drops")]
        public async Task<IActionResult> GetPriceDrops()
        {
            var priceHistories = await _priceHistoryRepository.GetPriceDropsAsync();
            return Ok(priceHistories);
        }

        [HttpGet("latest/{productId}")]
        public async Task<IActionResult> GetLatestPrice(string productId)
        {
            var priceHistory = await _priceHistoryRepository.GetLatestPriceAsync(productId);
            return priceHistory != null ? Ok(priceHistory) : NotFound();
        }
    }
}
