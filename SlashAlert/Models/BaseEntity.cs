using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public abstract class BaseEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("created_date")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [JsonPropertyName("created_by_id")]
        public string CreatedById { get; set; } = string.Empty;

        [JsonPropertyName("is_sample")]
        public string IsSample { get; set; } = string.Empty;
    }
}