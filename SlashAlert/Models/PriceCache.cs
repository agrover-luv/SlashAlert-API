using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public class PriceCache : BaseEntity
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;

        [JsonPropertyName("original_price")]
        public string OriginalPrice { get; set; } = string.Empty;

        [JsonPropertyName("discount_amount")]
        public string DiscountAmount { get; set; } = string.Empty;

        [JsonPropertyName("calculation_details")]
        public string CalculationDetails { get; set; } = string.Empty;

        [JsonPropertyName("product_name_found")]
        public string ProductNameFound { get; set; } = string.Empty;

        [JsonPropertyName("last_checked")]
        public string LastChecked { get; set; } = string.Empty;
    }
}