using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public class PriceHistory : BaseEntity
    {
        [BsonElement("product_id")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;

        [BsonElement("date")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [BsonElement("change_percentage")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("change_percentage")]
        public string ChangePercentage { get; set; } = string.Empty;
    }
}