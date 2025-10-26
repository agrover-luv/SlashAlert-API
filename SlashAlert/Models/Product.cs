using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public class Product : BaseEntity
    {
        [BsonElement("name")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("url")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [BsonElement("retailer")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("retailer")]
        public string Retailer { get; set; } = string.Empty;

        [BsonElement("purchased_date")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("purchased_date")]
        public string PurchasedDate { get; set; } = string.Empty;

        [BsonElement("current_price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("current_price")]
        public string CurrentPrice { get; set; } = string.Empty;

        [BsonElement("original_price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("original_price")]
        public string OriginalPrice { get; set; } = string.Empty;

        [BsonElement("target_price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("target_price")]
        public string TargetPrice { get; set; } = string.Empty;

        [BsonElement("target_price_type")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("target_price_type")]
        public string TargetPriceType { get; set; } = string.Empty;

        [BsonElement("target_price_percentage")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("target_price_percentage")]
        public string TargetPricePercentage { get; set; } = string.Empty;

        [BsonElement("image_url")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("category")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("is_active")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("is_active")]
        public string IsActive { get; set; } = string.Empty;

        [BsonElement("deleted")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("deleted")]
        public string Deleted { get; set; } = string.Empty;

        [BsonElement("deleted_at")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("deleted_at")]
        public string DeletedAt { get; set; } = string.Empty;

        [BsonElement("last_checked")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("last_checked")]
        public string LastChecked { get; set; } = string.Empty;

        [BsonElement("memory_size")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("memory_size")]
        public string MemorySize { get; set; } = string.Empty;

        [BsonElement("storage_size")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("storage_size")]
        public string StorageSize { get; set; } = string.Empty;

        [BsonElement("processor_type")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("processor_type")]
        public string ProcessorType { get; set; } = string.Empty;

        [BsonElement("screen_size")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("screen_size")]
        public string ScreenSize { get; set; } = string.Empty;
    }
}