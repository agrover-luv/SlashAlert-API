using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for managing product data from multiple sources (Cosmos DB containers, SQL tables, or CSV files)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpGet("retailer/{retailer}")]
        public async Task<IActionResult> GetByRetailer(string retailer)
        {
            var products = await _productRepository.GetByRetailerAsync(retailer);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            return Ok(products);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return Ok(products);
        }

        [HttpGet("creator/{createdById}")]
        public async Task<IActionResult> GetByCreatedById(string createdById)
        {
            var products = await _productRepository.GetByCreatedByIdAsync(createdById);
            return Ok(products);
        }

        [HttpGet("price-range")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var products = await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice);
            return Ok(products);
        }
    }
}
