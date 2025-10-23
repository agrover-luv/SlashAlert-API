using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public class Product : BaseEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("retailer")]
        public string Retailer { get; set; } = string.Empty;

        [JsonPropertyName("purchased_date")]
        public string PurchasedDate { get; set; } = string.Empty;

        [JsonPropertyName("current_price")]
        public string CurrentPrice { get; set; } = string.Empty;

        [JsonPropertyName("original_price")]
        public string OriginalPrice { get; set; } = string.Empty;

        [JsonPropertyName("target_price")]
        public string TargetPrice { get; set; } = string.Empty;

        [JsonPropertyName("target_price_type")]
        public string TargetPriceType { get; set; } = string.Empty;

        [JsonPropertyName("target_price_percentage")]
        public string TargetPricePercentage { get; set; } = string.Empty;

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("is_active")]
        public string IsActive { get; set; } = string.Empty;

        [JsonPropertyName("deleted")]
        public string Deleted { get; set; } = string.Empty;

        [JsonPropertyName("deleted_at")]
        public string DeletedAt { get; set; } = string.Empty;

        [JsonPropertyName("last_checked")]
        public string LastChecked { get; set; } = string.Empty;

        [JsonPropertyName("memory_size")]
        public string MemorySize { get; set; } = string.Empty;

        [JsonPropertyName("storage_size")]
        public string StorageSize { get; set; } = string.Empty;

        [JsonPropertyName("processor_type")]
        public string ProcessorType { get; set; } = string.Empty;

        [JsonPropertyName("screen_size")]
        public string ScreenSize { get; set; } = string.Empty;

        [JsonPropertyName("created_by")]
        public string CreatedBy { get; set; } = string.Empty;
    }
}