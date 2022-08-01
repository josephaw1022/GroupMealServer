using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace GroupMealApi.Models
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("ElectionRound")]
    public class ElectionRoundDBO : ElectionRound, IBase
    {
        [BsonId]
        [BsonElement("_id")]
        [JsonPropertyName("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; } = ObjectId.GenerateNewId().ToString();

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool Deleted { get; set; } = false;
    }
}