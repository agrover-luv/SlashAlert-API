using Microsoft.AspNetCore.Mvc;
using SlashAlert.Repositories.Interfaces;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for managing alert data from multiple sources (Cosmos DB containers, SQL tables, or CSV files)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertRepository _alertRepository;

        public AlertsController(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var alerts = await _alertRepository.GetAllAsync();
            return Ok(alerts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            return alert != null ? Ok(alert) : NotFound();
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(string productId)
        {
            var alerts = await _alertRepository.GetByProductIdAsync(productId);
            return Ok(alerts);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var alerts = await _alertRepository.GetByUserIdAsync(userId);
            return Ok(alerts);
        }

        [HttpGet("type/{alertType}")]
        public async Task<IActionResult> GetByAlertType(string alertType)
        {
            var alerts = await _alertRepository.GetByAlertTypeAsync(alertType);
            return Ok(alerts);
        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSentAlerts()
        {
            var alerts = await _alertRepository.GetSentAlertsAsync();
            return Ok(alerts);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentAlerts([FromQuery] int days = 30)
        {
            var alerts = await _alertRepository.GetRecentAlertsAsync(days);
            return Ok(alerts);
        }
    }
}
