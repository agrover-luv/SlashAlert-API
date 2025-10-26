using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SlashAlert.Api.Controllers
{
    /// <summary>
    /// Base controller that provides common functionality for all API controllers
    /// </summary>
    [ApiController]
    [Authorize] // Require authentication for all endpoints by default
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Gets the current authenticated user's email from the HttpContext
        /// </summary>
        protected string? CurrentUserEmail
        {
    get
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email");
                return emailClaim?.Value;
            }
        }

        return null;
    }
        }

        /// <summary>
        /// Gets the current authenticated user's email and throws an exception if not available
        /// </summary>
        protected string RequiredUserEmail
        {
            get
            {
                var email = CurrentUserEmail;
                if (string.IsNullOrEmpty(email))
                {
                    throw new UnauthorizedAccessException("User email not found in request context");
                }
                return email;
            }
        }

        /// <summary>
        /// Gets the current authenticated user's email for CreatedBy filtering and throws an exception if not available
        /// </summary>
        protected string RequiredCreatedBy
        {
            get
            {
                var email = CurrentUserEmail;
                if (string.IsNullOrEmpty(email))
                {
                    throw new UnauthorizedAccessException("User email not found in request context");
                }
                return email;
            }
        }

        /// <summary>
        /// Validates that the user is authenticated and has an email
        /// </summary>
        protected IActionResult ValidateAuthentication()
        {
            if (string.IsNullOrEmpty(CurrentUserEmail))
            {
                return Unauthorized("User email not found in authentication context");
            }
            return Ok();
        }
    }
}