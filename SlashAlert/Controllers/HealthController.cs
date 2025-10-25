using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Controller for application health monitoring and heartbeat checks
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Simple heartbeat endpoint to check if the API is alive
        /// </summary>
        /// <returns>Basic status information</returns>
        [HttpGet("heartbeat")]
        public IActionResult Heartbeat()
        {
            var response = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Message = "SlashAlert API is running"
            };

            return Ok(response);
        }

        /// <summary>
        /// Detailed health check with application information
        /// </summary>
        /// <returns>Detailed health status and application info</returns>
        [HttpGet("status")]
        public IActionResult Status()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "Unknown";
            
            var response = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Application = "SlashAlert API",
                Version = version,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                MachineName = Environment.MachineName,
                ProcessId = Environment.ProcessId,
                Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss")
            };

            return Ok(response);
        }

        /// <summary>
        /// Simple ping endpoint
        /// </summary>
        /// <returns>Pong response</returns>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { Message = "Pong", Timestamp = DateTime.UtcNow });
        }
    }
}