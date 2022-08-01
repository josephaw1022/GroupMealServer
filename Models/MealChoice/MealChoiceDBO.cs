using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace GroupMealApi.Models
{
    [BsonDiscriminator("MealChoice")]
    [BsonIgnoreExtraElements]
    public class MealChoiceDBO : MealChoice, IBase
    {
        [BsonId]
        [BsonElement("_id")]
        [JsonPropertyName("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("CreatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("Deleted")]
        public bool Deleted { get; set; } = false;
    }
}