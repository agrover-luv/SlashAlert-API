using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SlashAlert.Api.Controllers;

/// <summary>
/// Controller for testing Google JWT authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthTestController : ControllerBase
{
    /// <summary>
    /// Public endpoint that doesn't require authentication - useful for testing API availability
    /// </summary>
    /// <returns>Simple message indicating the API is working</returns>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult GetPublic()
    {
        return Ok(new { 
            message = "This is a public endpoint. No authentication required.",
            timestamp = DateTime.UtcNow,
            status = "success"
        });
    }

    /// <summary>
    /// Protected endpoint that requires valid Google JWT authentication
    /// </summary>
    /// <returns>User information extracted from the JWT token</returns>
    [HttpGet("protected")]
    [Authorize]
    public IActionResult GetProtected()
    {
        var userClaims = new
        {
            message = "Successfully authenticated with Google JWT!",
            timestamp = DateTime.UtcNow,
            user = new
            {
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value,
                name = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("name")?.Value,
                givenName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? User.FindFirst("given_name")?.Value,
                familyName = User.FindFirst(ClaimTypes.Surname)?.Value ?? User.FindFirst("family_name")?.Value,
                picture = User.FindFirst("picture")?.Value,
                issuer = User.FindFirst("iss")?.Value,
                audience = User.FindFirst("aud")?.Value,
                subject = User.FindFirst("sub")?.Value
            },
            allClaims = User.Claims.Select(c => new { type = c.Type, value = c.Value }).ToList()
        };

        return Ok(userClaims);
    }

    /// <summary>
    /// Endpoint to get current user profile information
    /// </summary>
    /// <returns>Current user profile data from JWT claims</returns>
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var profile = new
        {
            userId = User.FindFirst("sub")?.Value,
            email = User.FindFirst("email")?.Value,
            emailVerified = bool.TryParse(User.FindFirst("email_verified")?.Value, out var verified) && verified,
            name = User.FindFirst("name")?.Value,
            givenName = User.FindFirst("given_name")?.Value,
            familyName = User.FindFirst("family_name")?.Value,
            picture = User.FindFirst("picture")?.Value,
            locale = User.FindFirst("locale")?.Value,
            issuer = User.FindFirst("iss")?.Value,
            issuedAt = User.FindFirst("iat")?.Value,
            expiresAt = User.FindFirst("exp")?.Value
        };

        return Ok(profile);
    }

    /// <summary>
    /// Endpoint that requires specific Google OAuth policy
    /// </summary>
    /// <returns>Message confirming Google OAuth authentication</returns>
    [HttpGet("google-only")]
    [Authorize(Policy = "RequireGoogleAuth")]
    public IActionResult GetGoogleOnly()
    {
        return Ok(new
        {
            message = "This endpoint specifically requires Google OAuth authentication!",
            timestamp = DateTime.UtcNow,
            googleUser = User.FindFirst("email")?.Value
        });
    }
}