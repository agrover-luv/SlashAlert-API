using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public class Review : BaseEntity
    {
        [BsonElement("title")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("content")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("rating")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [BsonElement("user_name")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("user_name")]
        public string UserName { get; set; } = string.Empty;

        [BsonElement("user_location")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("user_location")]
        public string UserLocation { get; set; } = string.Empty;

        [BsonElement("is_verified")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("is_verified")]
        public string IsVerified { get; set; } = string.Empty;

        [BsonElement("helpful_count")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("helpful_count")]
        public string HelpfulCount { get; set; } = string.Empty;
    }
}