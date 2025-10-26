using Microsoft.AspNetCore.Mvc;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for managing product data from multiple sources (Cosmos DB containers, SQL tables, or CSV files)
    /// </summary>
    [Route("api/[controller]")]
    public class ProductsController : BaseApiController
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _productRepository.GetAllAsync(RequiredCreatedBy);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productRepository.GetByIdAsync(id, RequiredCreatedBy);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpGet("retailer/{retailer}")]
        public async Task<IActionResult> GetByRetailer(string retailer)
        {
            var products = await _productRepository.GetByRetailerAsync(retailer, RequiredCreatedBy);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category, RequiredCreatedBy);
            return Ok(products);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts()
        {
            var products = await _productRepository.GetActiveProductsAsync(RequiredCreatedBy);
            return Ok(products);
        }

        [HttpGet("creator/{createdBy}")]
        public async Task<IActionResult> GetByCreatedBy(string createdBy)
        {
            var products = await _productRepository.GetByCreatedByAsync(createdBy);
            return Ok(products);
        }

        [HttpGet("price-range")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var products = await _productRepository.GetByPriceRangeAsync(minPrice, maxPrice, RequiredCreatedBy);
            return Ok(products);
        }
    }
}
