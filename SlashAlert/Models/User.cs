using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public class User : BaseEntity
    {
        [BsonElement("partitionKey")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("partitionKey")]
        public string PartitionKey { get; set; } = string.Empty;

        // OAuth provider identifier (sub claim)
        [BsonElement("sub")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;

        // Provider name (google, facebook, microsoft, etc.)
        [BsonElement("provider")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("provider")]
        public string Provider { get; set; } = string.Empty;

        // Email from OAuth token
        [BsonElement("email")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("email_verified")]
        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

        // Name from OAuth token
        [BsonElement("name")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // Given name (first name)
        [BsonElement("given_name")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;

        // Family name (last name)
        [BsonElement("family_name")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = string.Empty;

        // Profile picture URL
        [BsonElement("picture")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        // Locale from OAuth token
        [BsonElement("locale")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;

        // When the user was created in our system
        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        // Last time user authenticated
        [BsonElement("last_login")]
        [JsonPropertyName("last_login")]
        public DateTime? LastLogin { get; set; }

        // Whether the user is active in our system
        [BsonElement("is_active")]
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; } = true;

        // OAuth token expiration (if needed for refresh)
        [BsonElement("token_expires_at")]
        [JsonPropertyName("token_expires_at")]
        public DateTime? TokenExpiresAt { get; set; }
    }
}