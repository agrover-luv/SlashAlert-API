using SlashAlert.Services;
using System.Security.Claims;

namespace SlashAlert.Middleware;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtAuthenticationMiddleware> _logger;

    public JwtAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<JwtAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractTokenFromRequest(context.Request);

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                // Resolve the scoped service within the request context
                var tokenValidationService = context.RequestServices.GetRequiredService<IGoogleTokenValidationService>();
                var principal = await tokenValidationService.ValidateTokenAsync(token);
                
                if (principal != null)
                {
                    context.User = principal;
                    _logger.LogInformation("Successfully authenticated user: {UserId}", 
                        principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown");
                }
                else
                {
                    _logger.LogWarning("Token validation failed for request to {Path}", context.Request.Path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token for request to {Path}", context.Request.Path);
            }
        }

        await _next(context);
    }

    private static string? ExtractTokenFromRequest(HttpRequest request)
    {
        // Check Authorization header
        var authHeader = request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Check query parameter as fallback
        if (request.Query.TryGetValue("access_token", out var token))
        {
            return token.FirstOrDefault();
        }

        return null;
    }
}