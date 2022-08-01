using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace GroupMealApi.Models
{
    public class GroupDTO : Group
    {
        [Key]
        [BsonId]
        [BsonElement("_id")]
        [JsonPropertyName("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
    }
}