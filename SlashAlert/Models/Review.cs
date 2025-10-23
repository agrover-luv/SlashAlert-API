using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public class Review : BaseEntity
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("user_name")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("user_location")]
        public string UserLocation { get; set; } = string.Empty;

        [JsonPropertyName("is_verified")]
        public string IsVerified { get; set; } = string.Empty;

        [JsonPropertyName("helpful_count")]
        public string HelpfulCount { get; set; } = string.Empty;
    }
}