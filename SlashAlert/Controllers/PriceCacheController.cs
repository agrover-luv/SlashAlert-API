using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for managing price cache data from multiple sources (Cosmos DB containers, SQL tables, or CSV files)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class PriceCacheController : ControllerBase
    {
        private readonly IPriceCacheRepository _priceCacheRepository;

        public PriceCacheController(IPriceCacheRepository priceCacheRepository)
        {
            _priceCacheRepository = priceCacheRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var priceCaches = await _priceCacheRepository.GetAllAsync();
            return Ok(priceCaches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var priceCache = await _priceCacheRepository.GetByIdAsync(id);
            return priceCache != null ? Ok(priceCache) : NotFound();
        }

        [HttpGet("url")]
        public async Task<IActionResult> GetByUrl([FromQuery] string url)
        {
            var priceCache = await _priceCacheRepository.GetByUrlAsync(url);
            return priceCache != null ? Ok(priceCache) : NotFound();
        }

        [HttpGet("product/{productName}")]
        public async Task<IActionResult> GetByProductName(string productName)
        {
            var priceCaches = await _priceCacheRepository.GetByProductNameAsync(productName);
            return Ok(priceCaches);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentlyChecked([FromQuery] int hours = 24)
        {
            var priceCaches = await _priceCacheRepository.GetRecentlyCheckedAsync(hours);
            return Ok(priceCaches);
        }

        [HttpGet("discounted")]
        public async Task<IActionResult> GetDiscountedItems()
        {
            var priceCaches = await _priceCacheRepository.GetDiscountedItemsAsync();
            return Ok(priceCaches);
        }
    }
}
