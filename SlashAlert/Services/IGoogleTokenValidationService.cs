using System.Security.Claims;

namespace SlashAlert.Services;

public interface IGoogleTokenValidationService
{
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    Task<bool> IsTokenValidAsync(string token);
}