using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SlashAlert.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SlashAlert.Services;

public class GoogleTokenValidationService : IGoogleTokenValidationService
{
    private readonly GoogleOAuthSettings _googleOAuthSettings;
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public GoogleTokenValidationService(IOptions<GoogleOAuthSettings> googleOAuthSettings)
    {
        _googleOAuthSettings = googleOAuthSettings.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
        
        // Configure the OpenID Connect configuration manager for Google
        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            "https://accounts.google.com/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            // Get the OpenID Connect configuration from Google
            var configuration = await _configurationManager.GetConfigurationAsync(CancellationToken.None);

            // Set up token validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = _googleOAuthSettings.ValidIssuers,
                
                ValidateAudience = !_googleOAuthSettings.DisableAudienceValidation,
                ValidAudiences = _googleOAuthSettings.ValidAudiences,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = configuration.SigningKeys,
                
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes clock skew
                
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            // Validate the token
            var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Additional validation: ensure it's a JWT token
            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                return null;
            }

            // Verify the algorithm
            if (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (SecurityTokenException)
        {
            // Token validation failed
            return null;
        }
        catch (Exception)
        {
            // Any other exception during validation
            return null;
        }
    }

    public async Task<bool> IsTokenValidAsync(string token)
    {
        var principal = await ValidateTokenAsync(token);
        return principal != null;
    }
}