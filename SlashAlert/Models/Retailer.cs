using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public class Retailer : BaseEntity
    {
        [BsonElement("name")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("label")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [BsonElement("price_guarantee_days")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("price_guarantee_days")]
        public string PriceGuaranteeDays { get; set; } = string.Empty;
    }
}