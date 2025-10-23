using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SlashAlert.Api.Services;

namespace SlashAlert.Api.Controllers
{
    [ApiController]
    [Route("api/llm")]
    public class LlmProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LlmProxyOptions _options;

        public LlmProxyController(IHttpClientFactory httpClientFactory, IOptions<LlmProxyOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public class ProxyRequest
        {
            public string? Prompt { get; set; }
            public object? Messages { get; set; }
            public string? Model { get; set; }
            public int? MaxTokens { get; set; }
            public double? Temperature { get; set; }
        }

        [HttpPost("proxy")]
        public async Task<IActionResult> Proxy([FromBody] ProxyRequest req)
        {
            // Prefer OpenAI if configured
            if (!string.IsNullOrWhiteSpace(_options.OpenAiApiKey))
            {
                var client = _httpClientFactory.CreateClient("LlmProxyClient");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.OpenAiApiKey);

                var body = new Dictionary<string, object>();
                body["model"] = req.Model ?? "gpt-3.5-turbo";

                if (req.Messages != null)
                {
                    body["messages"] = req.Messages;
                }
                else
                {
                    body["messages"] = new[] { new { role = "user", content = req.Prompt ?? string.Empty } };
                }

                if (req.MaxTokens.HasValue) body["max_tokens"] = req.MaxTokens.Value;
                if (req.Temperature.HasValue) body["temperature"] = req.Temperature.Value;

                var json = JsonSerializer.Serialize(body);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await client.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);
                var respText = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    return StatusCode((int)resp.StatusCode, respText);
                }
                using var doc = JsonDocument.Parse(respText);
                return Ok(JsonSerializer.Deserialize<object>(respText));
            }

            // Fallback: if Hugging Face key available
            if (!string.IsNullOrWhiteSpace(_options.HuggingFaceApiKey))
            {
                var client = _httpClientFactory.CreateClient("LlmProxyClient");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.HuggingFaceApiKey);
                var hfModel = _options.HuggingFaceModel ?? "gpt2";
                var payload = new Dictionary<string, object> { ["inputs"] = req.Prompt ?? string.Empty };
                var httpContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var resp = await client.PostAsync($"https://api-inference.huggingface.co/models/{hfModel}", httpContent);
                var respText = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    return StatusCode((int)resp.StatusCode, respText);
                }
                return Ok(JsonSerializer.Deserialize<object>(respText));
            }

            return BadRequest(new { error = "No LLM provider configured on the server. Set LlmProxy:OpenAiApiKey or LlmProxy:HuggingFaceApiKey in configuration or environment." });
        }
    }
}
