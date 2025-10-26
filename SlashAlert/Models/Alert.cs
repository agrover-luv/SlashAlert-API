using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public class Alert : BaseEntity
    {
        [BsonElement("product_id")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("user_id")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("alert_type")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("alert_type")]
        public string AlertType { get; set; } = string.Empty;

        [BsonElement("trigger_price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("trigger_price")]
        public string TriggerPrice { get; set; } = string.Empty;

        [BsonElement("previous_price")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("previous_price")]
        public string PreviousPrice { get; set; } = string.Empty;

        [BsonElement("message")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("email_sent")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("email_sent")]
        public string EmailSent { get; set; } = string.Empty;

        [BsonElement("sms_sent")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("sms_sent")]
        public string SmsSent { get; set; } = string.Empty;

        [BsonElement("sent_at")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("sent_at")]
        public string SentAt { get; set; } = string.Empty;
    }
}