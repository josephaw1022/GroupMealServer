using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupMealApi.Models
{
    public class MealChoiceJoinGroup
    {
        [Key]
        [BsonId]
        [BsonElement("_id")]
        [JsonPropertyName("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;


        public string Name { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public string? ImageUrl { get; set; } = null;

        public string? MenuUrl { get; set; } = null;

        public string GroupId { get; set; } = null!;
    }
}
