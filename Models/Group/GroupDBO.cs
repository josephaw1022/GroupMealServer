using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace GroupMealApi.Models
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("Group")]
    public class GroupDBO : Group
    {
        [Key]
        [BsonId]
        [BsonElement("_id")]
        [JsonPropertyName("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; } = ObjectId.GenerateNewId().ToString();

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool Deleted { get; set; } = false;

    }
}
