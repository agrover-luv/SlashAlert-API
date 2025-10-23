using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public class Retailer : BaseEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("price_guarantee_days")]
        public string PriceGuaranteeDays { get; set; } = string.Empty;
    }
}