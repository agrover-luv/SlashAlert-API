using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public class PriceHistory : BaseEntity
    {
        [JsonPropertyName("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("change_percentage")]
        public string ChangePercentage { get; set; } = string.Empty;
    }
}