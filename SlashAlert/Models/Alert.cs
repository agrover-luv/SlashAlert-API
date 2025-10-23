using System.Text.Json.Serialization;

namespace SlashAlert.Models
{
    public class Alert : BaseEntity
    {
        [JsonPropertyName("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("alert_type")]
        public string AlertType { get; set; } = string.Empty;

        [JsonPropertyName("trigger_price")]
        public string TriggerPrice { get; set; } = string.Empty;

        [JsonPropertyName("previous_price")]
        public string PreviousPrice { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("email_sent")]
        public string EmailSent { get; set; } = string.Empty;

        [JsonPropertyName("sms_sent")]
        public string SmsSent { get; set; } = string.Empty;

        [JsonPropertyName("sent_at")]
        public string SentAt { get; set; } = string.Empty;
    }
}