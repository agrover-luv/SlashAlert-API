using Microsoft.AspNetCore.Mvc;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RetailerExportController : ControllerBase
    {
        private readonly IRetailerRepository _retailerRepository;

        public RetailerExportController(IRetailerRepository retailerRepository)
        {
            _retailerRepository = retailerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var retailers = await _retailerRepository.GetAllAsync();
            return Ok(retailers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var retailer = await _retailerRepository.GetByIdAsync(id);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var retailer = await _retailerRepository.GetByNameAsync(name);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpGet("price-guarantee/{minDays}")]
        public async Task<IActionResult> GetByPriceGuarantee(int minDays)
        {
            var retailers = await _retailerRepository.GetByPriceGuaranteeDaysAsync(minDays);
            return Ok(retailers);
        }
    }
}
