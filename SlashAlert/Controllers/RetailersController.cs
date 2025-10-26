using Microsoft.AspNetCore.Mvc;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for managing retailer data from multiple sources (Cosmos DB containers, SQL tables, or CSV files)
    /// </summary>
    [Route("api/[controller]")]
    public class RetailersController : BaseApiController
    {
        private readonly IRetailerRepository _retailerRepository;

        public RetailersController(IRetailerRepository retailerRepository)
        {
            _retailerRepository = retailerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var retailers = await _retailerRepository.GetAllAsync(RequiredUserEmail);
            return Ok(retailers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var retailer = await _retailerRepository.GetByIdAsync(id, RequiredUserEmail);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var retailer = await _retailerRepository.GetByNameAsync(name, RequiredUserEmail);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpGet("price-guarantee/{minDays}")]
        public async Task<IActionResult> GetByPriceGuarantee(int minDays)
        {
            var retailers = await _retailerRepository.GetByPriceGuaranteeDaysAsync(minDays, RequiredUserEmail);
            return Ok(retailers);
        }
    }
}
