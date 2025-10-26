using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public class PriceCache : BaseEntity
    {
        [BsonElement("url")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;

        [BsonElement("original_price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("original_price")]
        public string OriginalPrice { get; set; } = string.Empty;

        [BsonElement("discount_amount")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("discount_amount")]
        public string DiscountAmount { get; set; } = string.Empty;

        [BsonElement("calculation_details")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("calculation_details")]
        public string CalculationDetails { get; set; } = string.Empty;

        [BsonElement("product_name_found")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("product_name_found")]
        public string ProductNameFound { get; set; } = string.Empty;

        [BsonElement("last_checked")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("last_checked")]
        public string LastChecked { get; set; } = string.Empty;
    }
}