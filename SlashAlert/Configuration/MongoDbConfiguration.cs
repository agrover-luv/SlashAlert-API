using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using SlashAlert.Models;

namespace SlashAlert.Configuration
{
    /// <summary>
    /// Custom serializer that can handle both DateTime and String BSON types and convert them to strings
    /// </summary>
    public class FlexibleStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();
            
            switch (bsonType)
            {
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.DateTime:
                    var dateTime = context.Reader.ReadDateTime();
                    // Convert BSON DateTime (milliseconds since epoch) to DateTime and then to string
                    var utcDateTime = DateTime.UnixEpoch.AddMilliseconds(dateTime);
                    return utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return string.Empty;
                default:
                    return context.Reader.ReadString(); // Fallback to string reading
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                context.Writer.WriteString(string.Empty);
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }

    /// <summary>
    /// Custom serializer that can handle numeric fields that might be stored as Int32, Double, or String
    /// </summary>
    public class FlexibleNumericStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();
            
            switch (bsonType)
            {
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Int32:
                    return context.Reader.ReadInt32().ToString();
                case BsonType.Int64:
                    return context.Reader.ReadInt64().ToString();
                case BsonType.Double:
                    return context.Reader.ReadDouble().ToString();
                case BsonType.Decimal128:
                    return context.Reader.ReadDecimal128().ToString();
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return string.Empty;
                default:
                    return context.Reader.ReadString(); // Fallback to string reading
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                context.Writer.WriteString(string.Empty);
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }

    /// <summary>
    /// Custom serializer that can handle Boolean fields that might be stored as Boolean or String
    /// </summary>
    public class FlexibleBooleanStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();
            
            switch (bsonType)
            {
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Boolean:
                    return context.Reader.ReadBoolean().ToString().ToLower();
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return string.Empty;
                default:
                    return context.Reader.ReadString(); // Fallback to string reading
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                context.Writer.WriteString(string.Empty);
            }
            else
            {
                context.Writer.WriteString(value);
            }
        }
    }

    /// <summary>
    /// Universal flexible serializer that can handle ANY BSON type and convert it to string
    /// This handles all possible MongoDB data types: String, Int32, Int64, Double, Boolean, DateTime, ObjectId, etc.
    /// </summary>
    public class UniversalStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();
            
            switch (bsonType)
            {
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Int32:
                    return context.Reader.ReadInt32().ToString();
                case BsonType.Int64:
                    return context.Reader.ReadInt64().ToString();
                case BsonType.Double:
                    return context.Reader.ReadDouble().ToString();
                case BsonType.Decimal128:
                    return context.Reader.ReadDecimal128().ToString();
                case BsonType.Boolean:
                    return context.Reader.ReadBoolean().ToString().ToLower();
                case BsonType.DateTime:
                    var dateTime = context.Reader.ReadDateTime();
                    var utcDateTime = DateTime.UnixEpoch.AddMilliseconds(dateTime);
                    return utcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                case BsonType.ObjectId:
                    return context.Reader.ReadObjectId().ToString();
                case BsonType.Binary:
                    var binaryData = context.Reader.ReadBinaryData();
                    return Convert.ToBase64String(binaryData.Bytes);
                case BsonType.Array:
                    // Read array and convert to JSON string
                    var arrayValue = BsonSerializer.Deserialize<BsonArray>(context.Reader);
                    return arrayValue.ToJson();
                case BsonType.Document:
                    // Read document and convert to JSON string
                    var documentValue = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
                    return documentValue.ToJson();
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return string.Empty;
                case BsonType.Undefined:
                    context.Reader.ReadUndefined();
                    return string.Empty;
                case BsonType.MinKey:
                    context.Reader.ReadMinKey();
                    return "MinKey";
                case BsonType.MaxKey:
                    context.Reader.ReadMaxKey();
                    return "MaxKey";
                case BsonType.Timestamp:
                    var timestamp = context.Reader.ReadTimestamp();
                    return timestamp.ToString();
                case BsonType.JavaScript:
                    return context.Reader.ReadJavaScript();
                case BsonType.JavaScriptWithScope:
                    var jsWithScope = context.Reader.ReadJavaScriptWithScope();
                    return jsWithScope;
                case BsonType.Symbol:
                    return context.Reader.ReadSymbol();
                case BsonType.RegularExpression:
                    var regex = context.Reader.ReadRegularExpression();
                    return $"/{regex.Pattern}/{regex.Options}";
                default:
                    // For any unknown type, skip and return empty
                    context.Reader.SkipValue();
                    return string.Empty;
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            context.Writer.WriteString(value ?? string.Empty);
        }
    }

    /// <summary>
    /// Custom serializer that can handle both DateTime and String BSON types and convert them to DateTime?
    /// </summary>
    public class FlexibleDateTimeSerializer : SerializerBase<DateTime?>
    {
        public override DateTime? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();
            
            switch (bsonType)
            {
                case BsonType.DateTime:
                    var dateTime = context.Reader.ReadDateTime();
                    // Convert BSON DateTime (milliseconds since epoch) to DateTime
                    return DateTime.UnixEpoch.AddMilliseconds(dateTime);
                case BsonType.String:
                    var dateString = context.Reader.ReadString();
                    if (string.IsNullOrEmpty(dateString))
                        return null;
                    if (DateTime.TryParse(dateString, out var parsedDate))
                        return parsedDate;
                    return null;
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                default:
                    return null;
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime? value)
        {
            if (value.HasValue)
            {
                context.Writer.WriteDateTime(BsonUtils.ToMillisecondsSinceEpoch(value.Value));
            }
            else
            {
                context.Writer.WriteNull();
            }
        }
    }

    /// <summary>
    /// Configuration for MongoDB serialization and conventions
    /// </summary>
    public static class MongoDbConfiguration
    {
        private static bool _isConfigured = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Configures MongoDB serialization conventions
        /// </summary>
        public static void Configure()
        {
            lock (_lock)
            {
                if (_isConfigured)
                    return;

                // Configure conventions for better field mapping
                var conventionPack = new ConventionPack
                {
                    // Ignore extra elements in the document that don't have corresponding properties
                    new IgnoreExtraElementsConvention(true),
                    // Use null values for missing elements
                    new IgnoreIfNullConvention(true)
                };

                ConventionRegistry.Register("SlashAlertConventions", conventionPack, t => true);

                _isConfigured = true;
            }
        }
    }
}