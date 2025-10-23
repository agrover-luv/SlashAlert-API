using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SlashAlert.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly Microsoft.IdentityModel.Protocols.IConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration> _oidcConfigManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IHttpClientFactory httpClientFactory,
                              IConfiguration configuration,
                              Microsoft.IdentityModel.Protocols.IConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration> oidcConfigManager,
                              ILogger<AuthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _oidcConfigManager = oidcConfigManager;
            _logger = logger;
        }

        // Exchange a Google ID token for a signed backend JWT. Payload: { idToken: string }
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleExchangeRequest req)
        {
            if (string.IsNullOrEmpty(req?.IdToken)) return BadRequest(new { error = "idToken is required" });

                // Validate the token using Google's OpenID Connect discovery and public keys (JWKS)
                var googleClientId = _configuration["Google:ClientId"];
                if (string.IsNullOrEmpty(googleClientId)) return StatusCode(500, new { error = "Google:ClientId not configured" });

                var openIdConfig = await _oidcConfigManager.GetConfigurationAsync(System.Threading.CancellationToken.None);
                var validationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "https://accounts.google.com",
                    ValidAudiences = new[] { googleClientId },
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

                var handler = new JwtSecurityTokenHandler();
                try
                {
                    var principal = handler.ValidateToken(req.IdToken, validationParameters, out var validatedToken);
                    // If ValidateToken succeeded, we'll extract fields below for issuance
                    _logger.LogInformation("Google ID token successfully validated for issuer {issuer}", principal?.FindFirst("iss")?.Value);
                }
                catch (Microsoft.IdentityModel.Tokens.SecurityTokenException stEx)
                {
                    _logger.LogWarning(stEx, "Failed to validate Google ID token");
                    return Unauthorized(new { error = "Invalid Google ID token", detail = stEx.Message });
                }

                // Re-parse to obtain email/name/sub for issuance (we can re-validate quickly)
                var validatedPrincipal = handler.ValidateToken(req.IdToken, validationParameters, out var _vt);
                var email = validatedPrincipal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                            ?? validatedPrincipal.FindFirst("email")?.Value;
                var name = validatedPrincipal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                            ?? validatedPrincipal.FindFirst("name")?.Value;
                var sub = validatedPrincipal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? validatedPrincipal.FindFirst("sub")?.Value;

            // Issue local JWT
            var jwtKey = _configuration["Jwt:Key"] ?? "dev-local-secret-please-change";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "slashalert-local";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "slashalert-local-audience";
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var emn) ? emn : 60;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, sub ?? email ?? ""),
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(ClaimTypes.Name, name ?? "")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt, email, name });
        }
    }

    public class GoogleExchangeRequest
    {
        public string? IdToken { get; set; }
    }
}
