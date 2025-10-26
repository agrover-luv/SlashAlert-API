using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SlashAlert.Models
{
    public abstract class BaseEntity
    {
        [BsonId]
        [BsonElement("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("created_date")]
        [BsonSerializer(typeof(SlashAlert.Configuration.FlexibleDateTimeSerializer))]
        [JsonPropertyName("created_date")]
        public DateTime? CreatedDate { get; set; }

        [BsonElement("updated_date")]
        [BsonSerializer(typeof(SlashAlert.Configuration.FlexibleDateTimeSerializer))]
        [JsonPropertyName("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [BsonElement("created_by_id")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("created_by_id")]
        public string CreatedById { get; set; } = string.Empty;

        [BsonElement("is_sample")]
        [BsonSerializer(typeof(SlashAlert.Configuration.UniversalStringSerializer))]
        [JsonPropertyName("is_sample")]
        public string IsSample { get; set; } = string.Empty;
    }
}