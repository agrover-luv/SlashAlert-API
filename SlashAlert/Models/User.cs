using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("partitionKey")]
        public string PartitionKey { get; set; } = string.Empty;

        // OAuth provider identifier (sub claim)
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;

        // Provider name (google, facebook, microsoft, etc.)
        [JsonPropertyName("provider")]
        public string Provider { get; set; } = string.Empty;

        // Email from OAuth token
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

        // Name from OAuth token
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // Given name (first name)
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;

        // Family name (last name)
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = string.Empty;

        // Profile picture URL
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        // Locale from OAuth token
        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;

        // When the user was created in our system
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        // Last time user authenticated
        [JsonPropertyName("last_login")]
        public DateTime? LastLogin { get; set; }

        // Whether the user is active in our system
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        // OAuth token expiration (if needed for refresh)
        [JsonPropertyName("token_expires_at")]
        public DateTime? TokenExpiresAt { get; set; }
    }
}